using Firmeza.Data;
using Firmeza.Data.Entities;
using Firmeza.Models;
using FirmezaAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace FirmezaAPI.Services;

public class ClientImportService : BaseImportExportService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Person> _userManager;

    public ClientImportService(ApplicationDbContext context, UserManager<Person> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ImportResultDto> ImportFromExcelAsync(Stream fileStream)
    {
        var result = new ImportResultDto();

        if (!ValidateExcelFile(fileStream, out var error))
        {
            result.Message = error;
            return result;
        }

        fileStream.Position = 0;

        // Configure EPPlus license
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];
        
        var rowCount = worksheet.Dimension?.Rows ?? 0;
        result.TotalRows = rowCount - 1; // Exclude header row

        // Find column indices
        var headers = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension?.Columns; col++)
        {
            var headerName = GetCellValue(worksheet, 1, col).ToLower();
            if (!string.IsNullOrEmpty(headerName))
            {
                headers[headerName] = col;
            }
        }

        // Process each row
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var client = await ProcessRowAsync(worksheet, row, headers, result);
                if (client != null)
                {
                    result.SuccessCount++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ImportErrorDto
                {
                    Row = row,
                    Field = "General",
                    Message = $"Error inesperado: {ex.Message}"
                });
                result.ErrorCount++;
            }
        }

        result.Message = $"Importación completada: {result.SuccessCount} éxitos, {result.ErrorCount} errores.";
        return result;
    }

    private async Task<Client?> ProcessRowAsync(ExcelWorksheet worksheet, int row, 
        Dictionary<string, int> headers, ImportResultDto result)
    {
        // Extract data from Excel row
        var name = GetColumnValue(worksheet, row, headers, "fullname", "nombrecompleto", "nombre", "name", "client", "cliente");
        var email = GetColumnValue(worksheet, row, headers, "email", "correo", "e-mail");
        var phone = GetColumnValue(worksheet, row, headers, "phone", "telefono", "teléfono", "tel", "celular");
        var address = GetColumnValue(worksheet, row, headers, "address", "direccion", "dirección", "dir", "domicilio");
        var document = GetColumnValue(worksheet, row, headers, "document", "documento", "dni", "cuit", "cuil");

        // Validate required fields
        if (string.IsNullOrWhiteSpace(name))
        {
            result.Errors.Add(new ImportErrorDto
            {
                Row = row,
                Field = "FullName",
                Message = "El nombre es obligatorio"
            });
            result.ErrorCount++;
            return null;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            result.Errors.Add(new ImportErrorDto
            {
                Row = row,
                Field = "Email",
                Message = "El email es obligatorio"
            });
            result.ErrorCount++;
            return null;
        }

        // Validate email format
        if (!IsValidEmail(email))
        {
            result.Errors.Add(new ImportErrorDto
            {
                Row = row,
                Field = "Email",
                Message = "El formato del email es inválido",
                Value = email
            });
            result.ErrorCount++;
            return null;
        }

        // Check if client exists (by email)
        var existingClient = await _context.Set<Client>()
            .FirstOrDefaultAsync(c => c.Email!.ToLower() == email.ToLower());

        if (existingClient != null)
        {
            // Update existing client
            existingClient.FullName = name;
            existingClient.Phone = phone;
            existingClient.PhoneNumber = phone; // Update Identity phone too
            existingClient.Address = address;
            if (!string.IsNullOrEmpty(document)) existingClient.Document = document;
            
            _context.Set<Client>().Update(existingClient);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Create new client (which is a Person/IdentityUser)
            var newClient = new Client
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = name,
                Phone = phone,
                PhoneNumber = phone,
                Address = address,
                Document = document,
                RegisterDate = DateTime.UtcNow
            };

            var defaultPassword = "Cliente123!"; // TODO: Generate random password or send email
            var createResult = await _userManager.CreateAsync(newClient, defaultPassword);

            if (!createResult.Succeeded)
            {
                result.Errors.Add(new ImportErrorDto
                {
                    Row = row,
                    Field = "Identity",
                    Message = $"Error al crear usuario: {string.Join(", ", createResult.Errors.Select(e => e.Description))}"
                });
                result.ErrorCount++;
                return null;
            }

            await _userManager.AddToRoleAsync(newClient, "Cliente");
            // No need to Add to context explicitly as CreateAsync does it
        }

        return existingClient ?? new Client { FullName = name };
    }

    private string GetColumnValue(ExcelWorksheet worksheet,  int row, 
        Dictionary<string, int> headers, params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            if (headers.TryGetValue(name.ToLower(), out var col))
            {
                return GetCellValue(worksheet, row, col);
            }
        }
        return string.Empty;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

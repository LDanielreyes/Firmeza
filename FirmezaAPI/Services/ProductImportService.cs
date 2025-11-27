using Firmeza.Data;
using Firmeza.Data.Entities;
using Firmeza.Models;
using FirmezaAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace FirmezaAPI.Services;

public class ProductImportService : BaseImportExportService
{
    private readonly ApplicationDbContext _context;

    public ProductImportService(ApplicationDbContext context)
    {
        _context = context;
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

        // Find column indices (case-insensitive search)
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
                var product = await ProcessRowAsync(worksheet, row, headers, result);
                if (product != null)
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

    private async Task<Product?> ProcessRowAsync(ExcelWorksheet worksheet, int row, 
        Dictionary<string, int> headers, ImportResultDto result)
    {
        // Extract data from Excel row
        var name = GetColumnValue(worksheet, row, headers, "productname", "nombre", "name", "producto");
        var priceStr = GetColumnValue(worksheet, row, headers, "price", "precio");
        var stockStr = GetColumnValue(worksheet, row, headers, "stock", "cantidad", "quantity");
        var description = GetColumnValue(worksheet, row, headers, "description", "descripcion", "descripción", "desc");

        // Validate required fields
        if (string.IsNullOrWhiteSpace(name))
        {
            result.Errors.Add(new ImportErrorDto
            {
                Row = row,
                Field = "ProductName",
                Message = "El nombre del producto es obligatorio"
            });
            result.ErrorCount++;
            return null;
        }

        if (string.IsNullOrWhiteSpace(priceStr))
        {
            result.Errors.Add(new ImportErrorDto
            {
                Row = row,
                Field = "Price",
                Message = "El precio es obligatorio"
            });
            result.ErrorCount++;
            return null;
        }

        if (!decimal.TryParse(priceStr, out var price) || price < 0)
        {
            result.Errors.Add(new ImportErrorDto
            {
                Row = row,
                Field = "Price",
                Message = "El precio debe ser un número positivo",
                Value = priceStr
            });
            result.ErrorCount++;
            return null;
        }

        int stock = 0;
        if (!string.IsNullOrWhiteSpace(stockStr) && (!int.TryParse(stockStr, out stock) || stock < 0))
        {
            result.Errors.Add(new ImportErrorDto
            {
                Row = row,
                Field = "Stock",
                Message = "El stock debe ser un número entero positivo",
                Value = stockStr
            });
            result.ErrorCount++;
            return null;
        }

        // Check if product exists (by name)
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());

        if (existingProduct != null)
        {
            // Update existing product
            existingProduct.Price = price;
            existingProduct.Stock = stock;
            existingProduct.Description = description;
            _context.Products.Update(existingProduct);
        }
        else
        {
            // Create new product
            var newProduct = new Product
            {
                Name = name,
                Price = price,
                Stock = stock,
                Description = description
            };
            _context.Products.Add(newProduct);
        }

        await _context.SaveChangesAsync();
        return existingProduct ?? new Product { Name = name };
    }

    private string GetColumnValue(ExcelWorksheet worksheet, int row, 
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
}

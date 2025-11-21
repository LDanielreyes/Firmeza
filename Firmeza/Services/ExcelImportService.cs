using Firmeza.Models;
using Microsoft.AspNetCore.Identity;

namespace Firmeza.Services;

using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Globalization;

// Asume que ImportResult y ColumnHeuristics están en el mismo namespace o accesibles.

public class ExcelImportService
{
    private readonly ApplicationDbContext _context;

    public ExcelImportService(ApplicationDbContext context)
    {
        _context = context;
        // Establecer la licencia de EPPlus.
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 
    }

    public async Task<ImportResult> ImportDataAsync(Stream fileStream)
    {
        var result = new ImportResult();
        var dataRows = new List<Dictionary<string, object>>();

        Dictionary<string, int>? columnMap;
        using (var package = new ExcelPackage(fileStream))
        {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null || worksheet.Dimension == null)
            {
                result.Errors.Add("The file is empty or contains no sheets/data.");
                return result;
            }
            
            columnMap = MapColumns(worksheet);

            if (!columnMap.Any())
            {
                result.Errors.Add("Could not identify any known data columns (Product or Client).");
                return result;
            }

            // 2. Lectura y Estandarización de Filas
            for (int row = worksheet.Dimension.Start.Row + 1; row <= worksheet.Dimension.End.Row; row++)
            {
                var rowData = new Dictionary<string, object>();
                
                foreach (var mapping in columnMap)
                {
                    // Almacena el valor de la celda de forma estándar, usando el nombre de la propiedad.
                    rowData[mapping.Key] = worksheet.Cells[row, mapping.Value].GetValue<object>();
                }
                if (rowData.Any()) 
                {
                     dataRows.Add(rowData);
                }
            }
        }

        await ProcessAndSaveData(dataRows, columnMap, result);
        
        return result;
    }
    
    private Dictionary<string, int> MapColumns(ExcelWorksheet worksheet)
    {
        var columnMap = new Dictionary<string, int>();
        var headerRow = worksheet.Cells[worksheet.Dimension.Start.Row, worksheet.Dimension.Start.Column, 
                                       worksheet.Dimension.Start.Row, worksheet.Dimension.End.Column];

        // Combina todas las heurísticas para un solo barrido
        var allKeywords = ColumnHeuristics.ProductKeywords
            .Concat(ColumnHeuristics.ClientKeywords)
            .ToDictionary(k => k.Key, v => v.Value);

        foreach (var cell in headerRow)
        {
            string header = cell.Text.Trim();
            if (string.IsNullOrEmpty(header)) continue;

            foreach (var keywordEntry in allKeywords)
            {
                // Verifica si el encabezado del archivo coincide con alguna palabra clave
                if (keywordEntry.Value.Any(k => k.Equals(header, StringComparison.OrdinalIgnoreCase)))
                {
                    // Mapea el nombre de propiedad estándar (e.g., "Name") con el índice de columna (e.g., 2)
                    columnMap[keywordEntry.Key] = cell.Start.Column;
                    break;
                }
            }
        }
        return columnMap;
    }
    
    // --- Lógica de Procesamiento y Persistencia ---
    private async Task ProcessAndSaveData(List<Dictionary<string, object>> dataRows, Dictionary<string, int> columnMap, ImportResult result)
    {
        int rowCount = 2; // Fila de Excel, empezando después del encabezado
        
        foreach (var rowData in dataRows)
        {
            // Determinar si hay suficientes campos para Productos
            bool canProcessProduct = ColumnHeuristics.ProductKeywords.Keys.All(key => columnMap.ContainsKey(key) && rowData.ContainsKey(key) && rowData[key] != null);
            
            // Determinar si hay suficientes campos para Clientes
            bool canProcessClient = ColumnHeuristics.ClientKeywords.Keys.All(key => columnMap.ContainsKey(key) && rowData.ContainsKey(key) && rowData[key] != null);

            // Intentar procesar Producto si tiene suficientes datos (Normalización: Separar y Relacionar)
            if (canProcessProduct)
            {
                await ProcessProduct(rowData, result, rowCount);
            }
            
            // Intentar procesar Cliente si tiene suficientes datos
            if (canProcessClient)
            {
                await ProcessClient(rowData, result, rowCount);
            }
            
            if (!canProcessProduct && !canProcessClient)
            {
                 result.Warnings.Add($"Row {rowCount}: Insufficient mandatory data to identify as Product or Client. Skipping.");
            }
            
            rowCount++;
        }
        
        // 4. Guardar Cambios (Si no hay errores críticos)
        if (result.Success)
        {
            await _context.SaveChangesAsync();
            result.Message = $"Import successful! Processed {dataRows.Count} rows.";
        }
        else
        {
            result.Message = $"Import failed due to {result.Errors.Count} critical errors. No changes were saved.";
        }
    }

    // --- Procesamiento de Entidades (Validación e Insert/Update) ---

    private async Task ProcessProduct(Dictionary<string, object> rowData, ImportResult result, int row)
    {
        // Normalización y Validación de Tipos
        string name = rowData["Name"]?.ToString()?.Trim();
        decimal price;
        int stock;
        
        if (!decimal.TryParse(rowData["Price"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out price) || price <= 0)
        {
            result.Errors.Add($"Row {row}: Product Price ('{rowData["Price"]}') must be a positive number.");
            return;
        }
        if (!int.TryParse(rowData["Stock"]?.ToString(), out stock) || stock < 0)
        {
            result.Errors.Add($"Row {row}: Product Stock ('{rowData["Stock"]}') must be a non-negative integer.");
            return;
        }

        // Busqueda: Actualizar o Insertar
        var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name.Equals(name));
        
        if (existingProduct != null)
        {
            // Actualizar: Asume que el stock importado se SUMA al stock existente.
            existingProduct.Price = price;
            existingProduct.Stock += stock; 
            _context.Products.Update(existingProduct);
            result.Log.Add($"Row {row}: Updated product '{name}'. New Stock: {existingProduct.Stock}");
        }
        else
        {
            // Insertar
            _context.Products.Add(new Product { Name = name, Price = price, Stock = stock });
            result.Log.Add($"Row {row}: Inserted new product '{name}'.");
        }
    }
    
    private async Task ProcessClient(Dictionary<string, object> rowData, ImportResult result, int row)
    {
        // Normalización y Validación de Datos
        string fullName = rowData["FullName"]?.ToString()?.Trim();
        string email = rowData["Email"]?.ToString()?.Trim();
        string phoneNumber = rowData["PhoneNumber"]?.ToString()?.Trim();

        // Validación de Datos Mandatorios (Email para normalizar la identidad)
        if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
        {
            result.Warnings.Add($"Row {row}: Client '{fullName}' has an invalid or missing Email. Skipping client creation.");
            return;
        }
        
        // Busqueda: Actualizar o Insertar
        var existingClient = await _context.Users.OfType<Client>().FirstOrDefaultAsync(c => c.Email.Equals(email));
        
        if (existingClient != null)
        {
            // Actualizar: Actualiza solo si la información es diferente
            if (!existingClient.FullName.Equals(fullName))
            {
                 existingClient.FullName = fullName;
                 _context.Users.Update(existingClient);
                 result.Log.Add($"Row {row}: Updated client name for email '{email}'.");
            }
        }
        else
        {
            // Insertar Nuevo Cliente (Necesita lógica de Identity, simplificado aquí)
            var newClient = new Client 
            { 
                FullName = fullName, 
                Email = email, 
                UserName = email, // Usar Email como UserName es una práctica común.
                PhoneNumber = phoneNumber,
                // PasswordHash debe ser inicializado por Identity, pero para el import directo lo marcamos como placeholder
                PasswordHash = new PasswordHasher<Person>().HashPassword(null, Guid.NewGuid().ToString()) 
            };
            _context.Users.Add(newClient);
            result.Log.Add($"Row {row}: Inserted new client '{fullName}'.");
        }
    }
}
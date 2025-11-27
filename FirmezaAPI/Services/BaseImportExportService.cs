using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FirmezaAPI.Services;

public class BaseImportExportService
{
    static BaseImportExportService()
    {
        // Configure QuestPDF license
        QuestPDF.Settings.License = LicenseType.Community;
    }

    protected static bool ValidateExcelFile(Stream fileStream, out string error)
    {
        error = string.Empty;

        try
        {
            // Configure EPPlus license before reading
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var package = new ExcelPackage(fileStream);
            if (package.Workbook.Worksheets.Count == 0)
            {
                error = "El archivo Excel no contiene hojas de trabajo.";
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            error = $"Error al leer el archivo Excel: {ex.Message}";
            return false;
        }
    }

    protected static string GetCellValue(ExcelWorksheet worksheet, int row, int col)
    {
        var cell = worksheet.Cells[row, col];
        return cell.Value?.ToString()?.Trim() ?? string.Empty;
    }

    protected static byte[] GenerateExcelFile(Action<ExcelPackage> configurePackage)
    {
        // Configure EPPlus for non-commercial use
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        using var package = new ExcelPackage();
        configurePackage(package);
        return package.GetAsByteArray();
    }

    protected static byte[] GeneratePdfFile(Action<IDocumentContainer> configureDocument)
    {
        return Document.Create(configureDocument).GeneratePdf();
    }
}

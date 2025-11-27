using Firmeza.Data;
using Firmeza.Data.Entities;
using Firmeza.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;

namespace FirmezaAPI.Services;

public class ProductExportService : BaseImportExportService
{
    private readonly ApplicationDbContext _context;

    public ProductExportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var products = await _context.Products.ToListAsync();

        return GenerateExcelFile(package =>
        {
            var worksheet = package.Workbook.Worksheets.Add("Productos");

            // Headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Nombre";
            worksheet.Cells[1, 3].Value = "Precio";
            worksheet.Cells[1, 4].Value = "Stock";
            worksheet.Cells[1, 5].Value = "Descripción";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            // Data
            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = product.Id;
                worksheet.Cells[row, 2].Value = product.Name;
                worksheet.Cells[row, 3].Value = product.Price;
                worksheet.Cells[row, 4].Value = product.Stock;
                worksheet.Cells[row, 5].Value = product.Description;
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();
        });
    }

    public async Task<byte[]> ExportToPdfAsync()
    {
        var products = await _context.Products.ToListAsync();

        return GeneratePdfFile(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text("Listado de Productos")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);  // ID
                            columns.RelativeColumn(3);   // Name
                            columns.RelativeColumn(1);   // Price
                            columns.RelativeColumn(1);   // Stock
                            columns.RelativeColumn(3);   // Description
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID").Bold();
                            header.Cell().Element(CellStyle).Text("Nombre").Bold();
                            header.Cell().Element(CellStyle).Text("Precio").Bold();
                            header.Cell().Element(CellStyle).Text("Stock").Bold();
                            header.Cell().Element(CellStyle).Text("Descripción").Bold();

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Background(Colors.Grey.Lighten3)
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(10)
                                    .AlignCenter()
                                    .AlignMiddle();
                            }
                        });

                        foreach (var product in products)
                        {
                            table.Cell().Element(DataCellStyle).Text(product.Id.ToString());
                            table.Cell().Element(DataCellStyle).Text(product.Name);
                            table.Cell().Element(DataCellStyle).Text($"${product.Price:F2}");
                            table.Cell().Element(DataCellStyle).Text(product.Stock.ToString());
                            table.Cell().Element(DataCellStyle).Text(product.Description ?? "");

                            static IContainer DataCellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(10);
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });
    }
}

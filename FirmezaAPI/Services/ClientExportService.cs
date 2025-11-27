using Firmeza.Data;
using Firmeza.Data.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;

namespace FirmezaAPI.Services;

public class ClientExportService : BaseImportExportService
{
    private readonly ApplicationDbContext _context;

    public ClientExportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var clients = await _context.Set<Client>().ToListAsync();

        return GenerateExcelFile(package =>
        {
            var worksheet = package.Workbook.Worksheets.Add("Clientes");

            // Headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Nombre Completo";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "Teléfono";
            worksheet.Cells[1, 5].Value = "Dirección";
            worksheet.Cells[1, 6].Value = "Documento";
            worksheet.Cells[1, 7].Value = "Fecha Registro";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 176, 80));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            // Data
            for (int i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = client.Id;
                worksheet.Cells[row, 2].Value = client.FullName;
                worksheet.Cells[row, 3].Value = client.Email;
                worksheet.Cells[row, 4].Value = client.Phone;
                worksheet.Cells[row, 5].Value = client.Address;
                worksheet.Cells[row, 6].Value = client.Document;
                worksheet.Cells[row, 7].Value = client.RegisterDate.ToString("yyyy-MM-dd");
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();
        });
    }

    public async Task<byte[]> ExportToPdfAsync()
    {
        var clients = await _context.Set<Client>().ToListAsync();

        return GeneratePdfFile(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header()
                    .Text("Listado de Clientes")
                    .SemiBold().FontSize(20).FontColor(Colors.Green.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(2);   // Name
                            columns.RelativeColumn(2);   // Email
                            columns.RelativeColumn(1);   // Phone
                            columns.RelativeColumn(2);   // Address
                            columns.RelativeColumn(1);   // Date
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID").Bold();
                            header.Cell().Element(CellStyle).Text("Nombre").Bold();
                            header.Cell().Element(CellStyle).Text("Email").Bold();
                            header.Cell().Element(CellStyle).Text("Teléfono").Bold();
                            header.Cell().Element(CellStyle).Text("Dirección").Bold();
                            header.Cell().Element(CellStyle).Text("Registro").Bold();

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Background(Colors.Grey.Lighten3)
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(8)
                                    .AlignCenter()
                                    .AlignMiddle();
                            }
                        });

                        foreach (var client in clients)
                        {
                            table.Cell().Element(DataCellStyle).Text(client.Id.ToString());
                            table.Cell().Element(DataCellStyle).Text(client.FullName);
                            table.Cell().Element(DataCellStyle).Text(client.Email ?? "");
                            table.Cell().Element(DataCellStyle).Text(client.Phone ?? "");
                            table.Cell().Element(DataCellStyle).Text(client.Address ?? "");
                            table.Cell().Element(DataCellStyle).Text(client.RegisterDate.ToString("dd/MM/yyyy"));

                            static IContainer DataCellStyle(IContainer container)
                            {
                                return container
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .PaddingVertical(5)
                                    .PaddingHorizontal(8);
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

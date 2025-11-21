using Firmeza.Models;
using Firmeza.Services;

namespace Firmeza.Pages.Admin.DataImport;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

public class DataImportModel : PageModel
{
    private readonly ExcelImportService _importService;

    public DataImportModel(ExcelImportService importService)
    {
        _importService = importService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Please select an Excel file.")]
    public IFormFile UploadedFile { get; set; }

    public ImportResult ImportResults { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || UploadedFile == null)
        {
            return Page();
        }

        // Validar tipo de archivo
        if (Path.GetExtension(UploadedFile.FileName).ToLower() != ".xlsx")
        {
            ModelState.AddModelError(nameof(UploadedFile), "Only .xlsx files are allowed.");
            return Page();
        }

        using (var stream = UploadedFile.OpenReadStream())
        {
            ImportResults = await _importService.ImportDataAsync(stream);
        }
        
        // Mostrar resultados y logs al administrador
        TempData["ImportMessage"] = ImportResults.Message;
        TempData["ImportLog"] = ImportResults.ToString(); 

        if (ImportResults.Success)
        {
            return RedirectToPage("./DataImportSuccess");
        }
        
        return Page(); // Regresa a la página de importación con los errores.
    }
}
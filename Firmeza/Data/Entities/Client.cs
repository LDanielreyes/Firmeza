using System.ComponentModel.DataAnnotations;
using Firmeza.Models;

namespace Firmeza.Data.Entities;

public class Client : Person
{
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Document { get; set; } = string.Empty; // UK en la BD

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    public byte Age { get; set; }
    public DateTime RegisterDate { get; set; } = DateTime.Now;
    
    public ICollection<Sale>? Sales { get; set; }
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public void ShowInfo()
    {
        Console.WriteLine($"Cliente: {FullName}, Documento: {Document}");
    }
}
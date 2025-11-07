using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Firmeza.Data.Entities;

public class Receipt
{
    [Key]
    public int Id { get; set; }
    
    public int? ProductId { get; set; } 
    public Product? Product { get; set; } 

    public int SaleId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal GrossTotal { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal IVATotal { get; set; }
    
    public List<Sale>? SaleLines { get; set; } 
    
    public void ShowInfo()
    {
        Console.WriteLine($"Receipt ID: {Id}, Total: {GrossTotal}");
    }
}
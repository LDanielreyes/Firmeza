using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Firmeza.Data.Entities;

public class Sale
{
    [Key]
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    
    public int ReceiptId { get; set; } 
    public Receipt Receipt { get; set; } = null!;
    
    public int ProductId { get; set; } 
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal NetTotal { get; set; }

    public void ShowInfo()
    {
        Console.WriteLine($"Sale Line ID: {Id}, Product ID: {ProductId}");
    }
}
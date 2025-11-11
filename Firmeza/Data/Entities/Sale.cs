using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Firmeza.Data.Entities
{
    public class Sale 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReceiptId { get; set; } 
        public Receipt Receipt { get; set; } = default!;
        [Required]
        public int ProductId { get; set; } 
        public Product Product { get; set; } = default!; 
        [Required]
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerUnit { get; set; } 

        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetTotal { get; set; } 
    }
}
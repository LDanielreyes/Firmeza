using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Firmeza.Data.Entities
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int ClientId { get; set; }
        public Client Client { get; set; } = default!; 

        [Required]
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrossTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal IvaTotal { get; set; }
        
        public List<Sale> SaleLines { get; set; } = new List<Sale>(); 
        
    }
}
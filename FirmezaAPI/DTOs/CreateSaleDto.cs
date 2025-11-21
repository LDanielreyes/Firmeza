using System.ComponentModel.DataAnnotations;

namespace FirmezaAPI.DTOs
{
    public class CreateSaleDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public List<CreateSaleItemDto> Items { get; set; } = new List<CreateSaleItemDto>();
    }

    public class CreateSaleItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}

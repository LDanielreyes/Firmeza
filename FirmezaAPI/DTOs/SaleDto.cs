namespace FirmezaAPI.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal NetTotal { get; set; }
    }
}

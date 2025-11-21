namespace FirmezaAPI.DTOs
{
    public class ReceiptDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal IvaTotal { get; set; }
        public List<SaleDto> SaleLines { get; set; } = new List<SaleDto>();
    }
}

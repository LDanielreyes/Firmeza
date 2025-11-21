namespace FirmezaAPI.DTOs
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public byte Age { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}

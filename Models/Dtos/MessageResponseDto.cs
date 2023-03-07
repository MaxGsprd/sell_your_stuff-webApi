namespace SellYourStuffWebApi.Models.Dtos
{
    public class MessageResponseDto
    {
        public int? Id { get; set; }
        public UserResponseForMessageDto? Author { get; set; }
        public UserResponseForMessageDto? Recipient { get; set; }
        public int AdId { get; set; }
        public bool isRead { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public DateTime? Date { get; set; }
    }
}

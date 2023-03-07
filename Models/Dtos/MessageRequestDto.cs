namespace SellYourStuffWebApi.Models.Dtos
{
    public class MessageRequestDto
    {
        public int? AuthorId { get; set; }
        public int? RecipientId { get; set; }
        public int AdId { get; set; }
        public bool isRead { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }
}

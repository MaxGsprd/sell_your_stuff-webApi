namespace SellYourStuffWebApi.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int? AuthorId { get; set; }
        public virtual User? Author { get; set; }
        public int? RecipientId { get; set; }
        public virtual User? Recipient { get; set; }
        public int AdId { get; set; }
        public bool isRead { get; set; } = false;
        public string? Title { get; set; }
        public string? Body { get; set; }
        public DateTime Date { get; set; }
    }
}

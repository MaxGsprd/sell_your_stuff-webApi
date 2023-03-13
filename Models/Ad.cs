namespace SellYourStuffWebApi.Models
{
    public class Ad
    {
        public Ad(string title, string description, int userId, int categoryId)
        {
            Title = title;
            Description = description;
            UserId = userId;
            CategoryId = categoryId;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Price { get; set; }
        public int UserId { get; set; }
        public virtual User? User { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public int ConditionId { get; set; }
        public virtual Condition? Condition { get; set; }
        public ICollection<Photo>? Photos { get; set; }
    }
}

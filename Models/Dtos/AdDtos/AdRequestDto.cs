namespace SellYourStuffWebApi.Models.Dtos.AdDtos
{
    public class AdRequestDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        public int Price { get; set; }
        public int? UserId { get; set; }
        public int? AddressId { get; set; }
        public int CategoryId { get; set; }
        public int? ConditionId { get; set; }
    }
}

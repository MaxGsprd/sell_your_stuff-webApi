namespace SellYourStuffWebApi.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int StreetNumber { get; set; }
        public string? StreetName { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Country { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}

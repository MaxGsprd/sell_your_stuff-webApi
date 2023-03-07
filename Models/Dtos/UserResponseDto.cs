namespace SellYourStuffWebApi.Models.Dtos
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string? phone { get; set; }
        public Role Role { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}

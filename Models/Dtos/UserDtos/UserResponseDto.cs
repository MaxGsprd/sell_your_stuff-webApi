namespace SellYourStuffWebApi.Models.Dtos.UserDtos
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string? phone { get; set; }
        public Roles Role { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}

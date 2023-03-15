namespace SellYourStuffWebApi.Models.Dtos.UserDtos
{
    public class UserLoginResponseDto
    {
        public int Id;
        public string Username { get; set; } = string.Empty;
        public string Token = string.Empty;
    }
}

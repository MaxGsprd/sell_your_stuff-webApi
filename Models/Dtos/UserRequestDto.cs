namespace SellYourStuffWebApi.Models.Dtos
{
    public class UserRequestDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? Phone { get; set; }
        public int RoleId { get; set; }
    }
}

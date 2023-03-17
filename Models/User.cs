using Microsoft.EntityFrameworkCore;

namespace SellYourStuffWebApi.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Email { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Phone { get; set; }
        public int RoleId { get; set; }
        public virtual Roles? Role { get; set; }
    }
}

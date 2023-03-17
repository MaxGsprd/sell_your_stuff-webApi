using Microsoft.EntityFrameworkCore;
using SellYourStuffWebApi.Models;

namespace SellYourStuffWebApi.Data
{
    public class SellYourStuffWebApiContext : DbContext
    {
        public SellYourStuffWebApiContext(DbContextOptions<SellYourStuffWebApiContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(user => user.Name).IsUnique();
            modelBuilder.Entity<Ad>().Navigation(ad => ad.Category).AutoInclude();
            modelBuilder.Entity<Ad>().Navigation(ad => ad.Condition).AutoInclude();
            modelBuilder.Entity<Ad>().Navigation(ad => ad.User).AutoInclude();
            modelBuilder.Entity<Ad>().Navigation(ad => ad.Photos).AutoInclude();
            modelBuilder.Entity<User>().Navigation(user => user.Role).AutoInclude();
            modelBuilder.Entity<Message>().Navigation(msg => msg.Author).AutoInclude();
            modelBuilder.Entity<Message>().Navigation(msg => msg.Recipient).AutoInclude();
        }

        public DbSet<Ad> Ad { get; set; } = default!;
        public DbSet<Address> Address { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Condition> Condition { get; set; } = default!;
        public DbSet<Message> Message { get; set; } = default!;
        public DbSet<Roles> Role { get; set; } = default!;
        public DbSet<User> User { get; set; } = default!;
    }
}

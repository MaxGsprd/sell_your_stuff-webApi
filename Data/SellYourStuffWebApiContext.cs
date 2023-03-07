using Microsoft.EntityFrameworkCore;
using SellYourStuffWebApi.Models;

namespace SellYourStuffWebApi.Data
{
    public class SellYourStuffWebApiContext : DbContext
    {
        public SellYourStuffWebApiContext(DbContextOptions<SellYourStuffWebApiContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<Ad>().Navigation(c => c.Category).AutoInclude();
            modelBuilder.Entity<Ad>().Navigation(c => c.Condition).AutoInclude();
            modelBuilder.Entity<Ad>().Navigation(u => u.User).AutoInclude();
            modelBuilder.Entity<User>().Navigation(r => r.Role).AutoInclude();
            modelBuilder.Entity<Message>().Navigation(a => a.Author).AutoInclude();
            modelBuilder.Entity<Message>().Navigation(a => a.Recipient).AutoInclude();
        }

        public DbSet<Ad> Ad { get; set; } = default!;
        public DbSet<Address> Address { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Condition> Condition { get; set; } = default!;
        public DbSet<Message> Message { get; set; } = default!;
        public DbSet<Role> Role { get; set; } = default!;
        public DbSet<User> User { get; set; } = default!;
    }
}

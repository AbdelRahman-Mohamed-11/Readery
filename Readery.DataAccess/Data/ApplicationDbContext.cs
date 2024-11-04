using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Readery.Core.Models;
using Readery.Core.Models.Identity;
using Readery.Web.Models;
using System.Reflection;

namespace Readery.DataAccess.Data
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext>
        options) : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options)
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().HasData(
        new Company
        {
            Id = 1,
            Name = "Tech Solution",
            StreetAddress = "123 Tech St",
            City = "Tech City",
            PostalCode = "12121",
            State = "IL",
            PhoneNumber = "6669990000"
        },
        new Company
        {
            Id = 2,
            Name = "Vivid Books",
            StreetAddress = "999 Vid St",
            City = "Vid City",
            PostalCode = "66666",
            State = "IL",
            PhoneNumber = "7779990000"
        },
        new Company
        {
            Id = 3,
            Name = "Readers Club",
            StreetAddress = "999 Main St",
            City = "Lala Land",
            PostalCode = "99999",
            State = "NY",
            PhoneNumber = "1113335555"
        }
    );

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}

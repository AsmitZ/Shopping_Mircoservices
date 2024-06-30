using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasData(new Product
        {
            ProductId = 1,
            Name = "Samosa",
            Price = 15,
            Description = "Punjabi Samosa",
            ImageURL = "",
            Category = "Appetizer"
        });
        
        modelBuilder.Entity<Product>().HasData(new Product
        {
            ProductId = 2,
            Name = "Paneer Tikka",
            Price = 20,
            Description = "Paneer Tikka with Marination",
            ImageURL = "",
            Category = "Appetizer"
        });
        
        modelBuilder.Entity<Product>().HasData(new Product
        {
            ProductId = 3,
            Name = "Pav Bhaji",
            Price = 10,
            Description = "Mumbai Pav Bhaji",
            ImageURL = "",
            Category = "Appetizer"
        });
        
        modelBuilder.Entity<Product>().HasData(new Product
        {
            ProductId = 4,
            Name = "Pani Puri",
            Price = 5,
            Description = "Mumbai Pani Puri",
            ImageURL = "",
            Category = "Appetizer"
        });
        
        modelBuilder.Entity<Product>().HasData(new Product
        {
            ProductId = 5,
            Name = "Masala Dosa",
            Price = 10,
            Description = "South Indian Masala Dosa",
            ImageURL = "",
            Category = "Main Course"
        });
        
        modelBuilder.Entity<Product>().HasData(new Product
        {
            ProductId = 6,
            Name = "Chicken Biryani",
            Price = 30,
            Description = "Hyderabadi Chicken Biryani",
            ImageURL = "",
            Category = "Main Course"
        });
        
        modelBuilder.Entity<Product>().HasData(new Product
        {
            ProductId = 7,
            Name = "Veg Biryani",
            Price = 15,
            Description = "Bombay Veg Biryani",
            ImageURL = "",
            Category = "Main Course"
        });
    }
}
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class MainDbContext: DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options): base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Admin> Admins { get; set; }
}

using Microsoft.EntityFrameworkCore;
using HotelServiceAPI.Models;

namespace HotelServiceAPI.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        // Fix: Removed references to non-existent 'Brand' property in the Service entity configuration.  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20).HasDefaultValue("User");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Cấu hình Blogs navigation: User.Blogs <-> Blog.AuthorId
                entity.HasMany(u => u.Blogs)
                      .WithOne(b => b.Author)
                      .HasForeignKey(b => b.AuthorId)
                      .HasConstraintName("FK_Blogs_Users_AuthorId");
            });

            // Blog configuration
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Quote).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(b => b.Author)
                      .WithMany(u => u.Blogs)
                      .HasForeignKey(b => b.AuthorId)
                      .HasConstraintName("FK_Blogs_Users_AuthorId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Service configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(s => s.CreatedBy)
                      .HasConstraintName("FK_Services_Users_CreatedBy")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Booking <-> User relationship
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired(false);
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Comment configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.GuestName).HasMaxLength(100);
                entity.Property(e => e.GuestEmail).HasMaxLength(100);
                // Blog relationship (required)
                entity.HasOne(e => e.Blog)
                      .WithMany()
                      .HasForeignKey(e => e.BlogId)
                      .OnDelete(DeleteBehavior.Cascade);
                // User relationship (optional)
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ContactMessage configuration
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }

        // Seed admin user
        public void SeedAdminUser()
        {
            if (!Users.Any(u => u.Email == "admin@hotel.com"))
            {
                Users.Add(new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@hotel.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                });
                SaveChanges();
            }
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using HotelServiceAPI.Models;

namespace HotelServiceAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        // public DbSet<Comment> Comments { get; set; } // TEMPORARILY DISABLED
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
            });

            // Blog configuration - EXPLICIT
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Quote).HasMaxLength(500);
                
                // Explicit AuthorId -> User relationship
                entity.HasOne(b => b.Author)
                      .WithMany()
                      .HasForeignKey(b => b.AuthorId)
                      .HasConstraintName("FK_Blogs_Users_AuthorId")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Comment configuration - COMPLETELY DISABLED
            // modelBuilder.Entity<Comment>(entity =>
            // {
            //     entity.HasKey(e => e.Id);
            //     entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            //     entity.Property(e => e.GuestName).HasMaxLength(100);
            //     entity.Property(e => e.GuestEmail).HasMaxLength(100);
            //     
            //     // Explicit property configurations
            //     entity.Property(e => e.BlogId).IsRequired();
            //     entity.Property(e => e.UserId).IsRequired(false);
            //     
            //     // Blog relationship only
            //     entity.HasOne(c => c.Blog)
            //           .WithMany()
            //           .HasForeignKey(c => c.BlogId)
            //           .HasConstraintName("FK_Comments_Blogs_BlogId")
            //           .OnDelete(DeleteBehavior.Cascade);
            // });

            // Service configuration - EXPLICIT
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                
                // Explicit CreatedBy -> User relationship  
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(s => s.CreatedBy)
                      .HasConstraintName("FK_Services_Users_CreatedBy")
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
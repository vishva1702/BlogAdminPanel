using BlogAdminPanel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogAdminPanel.Models.DTOs;

namespace BlogAdminPanel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SiteSettings> SiteSettings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");

            // Seed Admin User
            var adminUser = new User
            {
                Id = 1,
                UserName = "Admin",
                Email = "admin@blog.com",
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Admin@123"), // Replace with a strong password
                Role = "Admin",
                IsActive = true,
                CreatedOn = DateTime.Now,
                CreatedBy = "System",
                UpdatedOn = null,
                UpdatedBy = null,
                IsDeleted = false
            };

            modelBuilder.Entity<User>().HasData(adminUser);

            var defaultSettings = new SiteSettings
            {
                Id = 1,
                SiteName = "Default Blog",
                Tagline = "Welcome to Default Blog",
                Logo = "/images/default-logo.png", // Default logo path
                ContactEmail = "info@defaultblog.com",
                ContactPhone = "123-456-7890",
                SocialLinks = "facebook.com/defaultblog, twitter.com/defaultblog",
                CreatedOn = DateTime.Now,
                CreatedBy = "System"
            };

            modelBuilder.Entity<SiteSettings>().HasData(defaultSettings);
        }
    }
}//protected override void OnModelCreating(ModelBuilder modelBuilder)
     //{
     //    base.OnModelCreating(modelBuilder);

    //    modelBuilder.Entity<BlogPost>()
    //        .HasOne(bp => bp.Category)
    //        .WithMany(c => c.BlogPosts)
    //        .HasForeignKey(bp => bp.CategoryId)
    //        .OnDelete(DeleteBehavior.Restrict);

    //    modelBuilder.Entity<BlogPost>()
    //        .HasOne(bp => bp.Tag)
    //        .WithMany(t => t.BlogPosts)
    //        .HasForeignKey(bp => bp.TagId)
    //        .OnDelete(DeleteBehavior.Restrict);

    //    modelBuilder.Entity<Comment>()
    //        .HasOne(c => c.BlogPost)
    //        .WithMany(bp => bp.Comments)
    //        .HasForeignKey(c => c.BlogPostId)
    //        .OnDelete(DeleteBehavior.Cascade);

    //    modelBuilder.Entity<Category>()
    //        .HasIndex(c => c.Name)
    //        .IsUnique();

    //    modelBuilder.Entity<Tag>()
    //        .HasIndex(t => t.Name)
    //        .IsUnique();

    //    modelBuilder.Entity<User>()
    //        .HasIndex(u => u.Email)
    //        .IsUnique();
    //}

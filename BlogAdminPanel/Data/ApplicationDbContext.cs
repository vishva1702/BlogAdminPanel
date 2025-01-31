using BlogAdminPanel.Models;
using BlogAdminPanel.Models.BlogAdminPanel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BlogAdminPanel.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    
        public ApplicationDbContext()
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


            // Seed Admin User
            var adminUser = new User
            {
                Id = 1,
                UserName = "Admin",
                Email = "admin@blog.com",
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Admin@123"), // Replace with a strong password
                Role = "Admin",
                IsActive = true,
                Image = "/images/logo.png",
                CreatedOn = DateTime.Now,
                CreatedBy = "System",
                UpdatedOn = null,
                UpdatedBy = null,
                IsDeleted = false
            };

            var editorUser = new User
            {
                Id = 2,
                UserName = "Editor",
                Email = "editor@blog.com",
                PasswordHash = new PasswordHasher<User>().HashPassword(null, "Editor@123"), // Replace with a strong password
                Role = "Editor",
                IsActive = true,
                Image = "/images/editor.png",
                CreatedOn = DateTime.Now,
                CreatedBy = "System",
                UpdatedOn = null,
                UpdatedBy = null,
                IsDeleted = false
            };

            modelBuilder.Entity<User>().HasData(adminUser, editorUser);

            // Seed Default Site Settings
            var defaultSettings = new SiteSettings
            {
                Id = 1,
                SiteName = "Default Blog",
                Tagline = "Welcome to Default Blog",
                Logo = "/images/logo.png",
                ContactEmail = "info@defaultblog.com",
                ContactPhone = "123-456-7890",
                SocialLinks = "facebook.com/defaultblog, twitter.com/defaultblog",
                CreatedOn = DateTime.Now,
                CreatedBy = "System"
            };

            modelBuilder.Entity<SiteSettings>().HasData(defaultSettings);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.Entity is User || e.Entity is SiteSettings)
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is User user)
                    {
                        user.CreatedOn = DateTime.Now;
                        user.IsDeleted = false; // Ensure soft delete is set to false for new entities
                    }
                }

                if (entry.Entity is User modifiedUser)
                {
                    modifiedUser.UpdatedOn = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}

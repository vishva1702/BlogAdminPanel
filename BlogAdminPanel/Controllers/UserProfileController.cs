using BlogAdminPanel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;
using System;
using BlogAdminPanel.ViewModels;
using Microsoft.EntityFrameworkCore;
using BlogAdminPanel.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogAdminPanel.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserProfileController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: /UserProfile/Index
        public IActionResult Index()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
                if (user != null)
                {
                    return View(user); // Pass the user with the image path to the view
                }
            }

            return RedirectToAction("Login", "Account");
        }

        // POST: /UserProfile/Index
        [HttpPost]
        public async Task<IActionResult> Index(User updatedUser, IFormFile profileImage)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
                if (user != null)
                {
                    // Update user details
                    user.UserName = updatedUser.UserName;

                    // If a new profile image is uploaded, save it and update the user's image path
                    if (profileImage != null)
                    {
                        user.Image = await SaveProfileImage(profileImage);
                    }

                    // Save changes to the database
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Index"); // Redirect to the same Index action
                }
            }

            TempData["ErrorMessage"] = "Unable to update profile. Please try again.";
            return View(updatedUser); // Return to the same view with the model
        }

        // POST: /UserProfile/EditProfileImage
        [HttpPost]
        public async Task<IActionResult> EditProfileImage(IFormFile profileImage)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email != null && profileImage != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
                if (user != null)
                {
                    // Delete the existing profile image
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Image.TrimStart('/'));
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }

                    // Save the new profile image
                    user.Image = await SaveProfileImage(profileImage);

                    // Update user in the database
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Profile image updated successfully!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select an image to upload.";
            }

            return RedirectToAction("Index");
        }

        // Helper method to save the uploaded profile image
        private async Task<string> SaveProfileImage(IFormFile profileImage)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

            var uniqueFileName = $"{Guid.NewGuid()}_{profileImage.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueFileName}";
        }

        [HttpPost]
        public IActionResult RemoveProfileImage()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (email != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
                if (user != null)
                {
                    // Delete the image from the server
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Image.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }

                    // Reset user's profile image to default or null
                    user.Image = "/images/default-profile.png"; // Replace with a default image if necessary
                    _context.Users.Update(user);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Profile image removed successfully!";
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid input. Please check your data.";
                return RedirectToAction("Index");
            }

            // Get the logged-in user from the database
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Use PasswordHasher to verify the current password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return RedirectToAction("Index");
            }

            // Hash the new password and update it
            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            user.UpdatedOn = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction("Index");
        }
    }
}

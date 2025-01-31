using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogAdminPanel.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public ChangePasswordController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with validation errors
                return View("Index", model);
            }

            // Get the logged-in user from the database
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Verify the current password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View("Index", model);
            }

            // Check if the new password matches the confirm password
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "New password and confirm password do not match.");
                return View("Index", model);
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
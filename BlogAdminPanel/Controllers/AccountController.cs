using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace BlogAdminPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult VerifyEmail()
        {
            return View(new VerifyEmailViewModel());
        }

        [HttpPost]
        public IActionResult SendOTP(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("VerifyEmail", model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email address not found.");
                return View("VerifyEmail", model);
            }

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            TempData["OTP"] = otp;
            TempData["Email"] = model.Email;

            // Send OTP via email
            try
            {
                SendEmail(model.Email, "Your OTP for Password Reset", $"Your OTP is {otp}. It is valid for 10 minutes.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error sending OTP: " + ex.Message);
                return View("VerifyEmail", model);
            }

            return RedirectToAction("VerifyOTP");
        }

        public IActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTP(string OTP)
        {
            var storedOtp = TempData["OTP"]?.ToString();
            var email = TempData["Email"]?.ToString();

            if (string.IsNullOrEmpty(OTP))
            {
                ModelState.AddModelError("", "OTP is required.");
            }
            else if (storedOtp == null || storedOtp != OTP)
            {
                ModelState.AddModelError("", "Invalid or expired OTP.");
            }

            if (!ModelState.IsValid)
            {
                return View(); // Return to the view with error messages
            }

            TempData["VerifiedEmail"] = email;
            return RedirectToAction("ResetPassword");
        }


        public IActionResult ResetPassword()
        {
            var email = TempData["VerifiedEmail"]?.ToString();
            if (email == null)
            {
                return RedirectToAction("VerifyEmail");
            }

            return View(new ChangePasswordViewModel { Email = email });
        }

        [HttpPost]
        public IActionResult ResetPassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return to the view with error messages
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            // Hash the new password and update
            user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
            user.UpdatedOn = DateTime.Now;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating password: " + ex.Message);
                return View(model);
            }

            return RedirectToAction("Login");
        }


        private void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("havendesign3@gmail.com");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "tleawsujnotrruzv";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}

using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogAdminPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid login data." });
            }

            // Fetch user from the database using the Email address
            var user = _context.Users.FirstOrDefault(u => u.Email == loginModel.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Store login data in the database
            StoreLoginData(user.Email);

            // Generate JWT token
            var token = GenerateToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);

            // Redirect to UserController without specifying an action (defaults to Index)
            return RedirectToAction("Index", "User");
        }

        private void StoreLoginData(string email)
        {
            
            var loginHistory = new LoginHistory
            {
                UserEmail = email,
                LoginDate = DateTime.Now,
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"]
            };

            _context.LoginHistories.Add(loginHistory);
            _context.SaveChanges();
        }


        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "User") 
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
            var otp = new Random().Next(100000, 999999).ToString();
            TempData["OTP"] = otp;
            TempData["Email"] = model.Email;
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
                return View(); 
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

        // Method to send email (for OTP)
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

using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.ViewModels;
using Microsoft.AspNetCore.Identity;
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
            private readonly SignInManager<IdentityUser> _signInManager;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly ILogger<AccountController> _logger;

            public AccountController(ApplicationDbContext context, IConfiguration configuration, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<AccountController> logger)
            {
                _context = context;
                _passwordHasher = new PasswordHasher<User>();
                _configuration = configuration;
                _signInManager = signInManager;
                _userManager = userManager;
                _logger = logger;
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
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid login data.");
                    return BadRequest(new { message = "Invalid login data." });
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == loginModel.Email);

                if (user == null)
                {
                    _logger.LogWarning($"Login attempt with invalid email: {loginModel.Email}");
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);

                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning($"Failed login attempt for email: {loginModel.Email}");
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                StoreLoginData(user.Email);

                var token = GenerateToken(user);

                await SetAuthCookieAsync(token);

                _logger.LogInformation($"User {user.Email} logged in successfully.");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Login method: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
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

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Admin") // Set explicitly to "Admin"
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task SetAuthCookieAsync(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(24)
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                Response.Cookies.Delete("AuthToken");

                _logger.LogInformation("User logged out successfully.");
                return Redirect("/Account/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Logout method: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
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

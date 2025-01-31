using BlogAdminPanel.Models;
using BlogAdminPanel.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BlogAdminPanel.Data;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;

namespace BlogAdminPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;


        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();

        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                ViewData["ReturnUrl"] = returnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && !u.IsDeleted);

                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<User>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                    if (result == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim("ProfilePicture", user.Image ?? "~/assets/img/default-profile.jpg"),
                            new Claim(ClaimTypes.Role, user.Role)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            // Sign out the user by removing the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Remove the authentication token from cookies
            Response.Cookies.Delete("AuthToken");

            // Redirect to the returnUrl or the home page if no returnUrl is provided
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
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

            return View(new ResetPasswordViewModel { Email = email });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
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
            var fromAddress = new MailAddress("havendesign3@gmail.com", "BlogAdmin Support");
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
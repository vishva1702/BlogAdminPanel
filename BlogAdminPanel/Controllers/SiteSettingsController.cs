using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlogAdminPanel.Controllers
{
    public class SiteSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiteSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var settings = _context.SiteSettings.FirstOrDefault() ?? new SiteSettings();
            var settingsDto = new SiteSettingsDto
            {
                SiteName = settings.SiteName,
                Tagline = settings.Tagline,
                ContactEmail = settings.ContactEmail,
                ContactPhone = settings.ContactPhone,
                SocialLinks = settings.SocialLinks,
                Logo = settings.Logo
            };

            ViewData["LogoUrl"] = settings.Logo;
            return View(settingsDto);
        }

        [HttpPost]
        public IActionResult Index(SiteSettingsDto model)
        {
            if (ModelState.IsValid)
            {
                var settings = _context.SiteSettings.FirstOrDefault();

                if (settings == null)
                {
                    settings = new SiteSettings
                    {
                        CreatedOn = DateTime.Now,
                        CreatedBy = "Admin"
                    };
                    _context.SiteSettings.Add(settings);
                }

                // Update fields
                settings.SiteName = model.SiteName;
                settings.Tagline = model.Tagline;

                // Handle Logo Upload
                if (model.LogoFile != null && model.LogoFile.Length > 0)
                {
                    try
                    {
                        // Generate a unique file name to avoid overwrite
                        var fileName = Path.GetFileNameWithoutExtension(model.LogoFile.FileName);
                        var extension = Path.GetExtension(model.LogoFile.FileName);
                        var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine("wwwroot/img", uniqueFileName);

                        // Debugging: Check if file is being uploaded correctly
                        Console.WriteLine($"Uploading file: {filePath}");

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            model.LogoFile.CopyTo(stream);
                        }

                        settings.Logo = $"/img/{uniqueFileName}";

                        TempData["SuccessMessage"] = "Logo updated successfully!";

                        Console.WriteLine($"Logo URL: {settings.Logo}");
                    }
                    catch (Exception ex)
                    {
                        // Log the error to check if there's an issue with file uploading
                        Console.WriteLine($"Error uploading logo: {ex.Message}");
                        ModelState.AddModelError(string.Empty, "There was an error uploading the logo.");
                    }
                }

                settings.ContactEmail = model.ContactEmail;
                settings.ContactPhone = model.ContactPhone;
                settings.SocialLinks = model.SocialLinks;
                settings.UpdatedOn = DateTime.Now;

                _context.SiteSettings.Update(settings);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // If the model is not valid, return the view with error messages
            return View(model);
        }
    }
}




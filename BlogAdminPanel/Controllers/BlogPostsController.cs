using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace BlogAdminPanel.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogPostsController(ApplicationDbContext context)
        {
            _context = context;
        }


        //index

        public IActionResult Index()
        {
            var posts = _context.BlogPosts
                                .Where(bp => !bp.IsDeleted && bp.Status != "Draft" && bp.Status != "Archived") // Exclude Draft and Archived posts
                                .Include(bp => bp.Category)
                                .Include(bp => bp.Tag)
                                .ToList();

            foreach (var post in posts)
            {
                if (post.PublishedDate.HasValue && post.PublishedDate > DateTime.Now && post.Status != "Published")
                {
                    post.Status = "Upcoming";
                }

                if (post.PublishedDate.HasValue && post.PublishedDate <= DateTime.Now && post.Status != "Published")
                {
                    post.Status = "Published";
                    post.IsPublished = true;
                    post.PublishedDate = DateTime.Now;
                }
            }

            _context.SaveChanges();

            return View(posts);
        }

        // Create
        public IActionResult Create()
        {
            ViewBag.StatusOptions = GetStatusDropdown();
            ViewBag.CategoryOptions = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.TagOptions = new SelectList(_context.Tags, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogPostCreateDto dto)
        {
            try
            {
                // Check if the model state is valid
                if (!ModelState.IsValid)
                {
                    // Repopulate the dropdowns in case of errors
                    ViewBag.StatusOptions = GetStatusDropdown();
                    ViewBag.CategoryOptions = new SelectList(_context.Categories, "Id", "Name");
                    ViewBag.TagOptions = new SelectList(_context.Tags, "Id", "Name");
                    return View(dto); // Return the same view with the validation errors
                }


                string imagePath = null;

                // Handle file upload
                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dto.ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            dto.ImageFile.CopyTo(fileStream);
                        }

                        imagePath = "/images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the image: " + ex.Message;
                        return View(dto); // Return to the view with error message
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please upload an image.";
                    return View(dto); // Return to the view with error message
                }

                // Create a new BlogPost object
                var blogPost = new BlogPost
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    MetaTitle = dto.MetaTitle,
                    MetaDescription = dto.MetaDescription,
                    Keywords = dto.Keywords,
                    Status = dto.Status,
                    CreatedBy = "admin",
                    CreatedDate = DateTime.Now,
                    CategoryId = dto.CategoryId,
                    TagId = dto.TagId,
                    Image = imagePath,
                    IsDraft = dto.Status == "Draft",
                    IsPublished = dto.Status == "Published",
                    IsArchived = dto.Status == "Archived",
                    PublishedDate = dto.Status == "Published" ? dto.PublishDate : (DateTime?)null
                };

                // Adjust status for future publish dates
                if (blogPost.PublishedDate.HasValue && blogPost.PublishedDate > DateTime.Now)
                {
                    blogPost.Status = "Upcoming";
                }

                // Save to database
                try
                {
                    _context.BlogPosts.Add(blogPost);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving the blog post: " + ex.Message;
                    return View(dto); // Return to the view with error message
                }

                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while saving the category: " + ex.Message;
                TempData["InnerException"] = ex.InnerException?.Message; // Capture inner exception message
            }
            return RedirectToAction(nameof(Index));
        }


        // Edit
        public IActionResult Edit(int id)
        {
            var blogPost = _context.BlogPosts.FirstOrDefault(bp => bp.Id == id);

            if (blogPost == null)
                return NotFound();

            var dto = new BlogPostUpdateDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                MetaTitle = blogPost.MetaTitle,
                MetaDescription = blogPost.MetaDescription,
                Keywords = blogPost.Keywords,
                Status = blogPost.Status,
                UpdatedBy = blogPost.UpdatedBy,
                CategoryId = blogPost.CategoryId,
                TagId = blogPost.TagId
            };

            ViewBag.StatusOptions = GetStatusDropdown();
            ViewBag.CategoryOptions = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.TagOptions = new SelectList(_context.Tags, "Id", "Name");

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BlogPostUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusOptions = GetStatusDropdown();
                ViewBag.CategoryOptions = new SelectList(_context.Categories, "Id", "Name");
                ViewBag.TagOptions = new SelectList(_context.Tags, "Id", "Name");
                return View(dto);
            }

            var blogPost = _context.BlogPosts.FirstOrDefault(bp => bp.Id == dto.Id);
            if (blogPost == null)
                return NotFound();

            string imagePath = blogPost.Image;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (blogPost.Image != null)
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", blogPost.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    dto.ImageFile.CopyTo(fileStream);
                }

                imagePath = "/images/" + uniqueFileName;
            }

            blogPost.Title = dto.Title;
            blogPost.Content = dto.Content;
            blogPost.MetaTitle = dto.MetaTitle;
            blogPost.MetaDescription = dto.MetaDescription;
            blogPost.Keywords = dto.Keywords;
            blogPost.Status = dto.Status;
            blogPost.CategoryId = dto.CategoryId;
            blogPost.TagId = dto.TagId;
            blogPost.Image = imagePath;
            blogPost.UpdatedBy = dto.UpdatedBy;
            blogPost.UpdatedOn = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        // Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var blogpost = _context.BlogPosts.Find(id);
            if (blogpost == null || blogpost.IsDeleted)
                return NotFound();

            blogpost.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Archive - GET
        public IActionResult Archive()
        {
            var archivedPosts = _context.BlogPosts
                                        .Where(bp => bp.Status == "Archived" && !bp.IsDeleted)
                                        .Include(bp => bp.Category)
                                        .Include(bp => bp.Tag)
                                        .ToList();

            return View(archivedPosts);
        }

        // Archive - POST (Optional if triggered elsewhere)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Archive(int id)
        {
            var blogPost = _context.BlogPosts.FirstOrDefault(bp => bp.Id == id);

            if (blogPost == null)
                return NotFound();

            blogPost.Status = "Archived";
            blogPost.IsDraft = false;
            blogPost.IsPublished = false;
            blogPost.IsArchived = true;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult UnArchive(int id)
        {
            var blogPost = _context.BlogPosts.FirstOrDefault(bp => bp.Id == id);

            if (blogPost == null)
                return NotFound();

            return View(blogPost);
        }

        // UnArchive - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UnArchiveConfirmed(int id)
        {
            var blogPost = _context.BlogPosts.FirstOrDefault(bp => bp.Id == id);

            if (blogPost == null)
                return NotFound();

            blogPost.Status = "Published";
            blogPost.IsArchived = false;
            blogPost.IsPublished = true;

            _context.SaveChanges();

            return RedirectToAction(nameof(Archive)); // Redirect to Archive view
        }



        // Publish code
        public IActionResult Publish(int id)
        {
            var blogPost = _context.BlogPosts
                                   .Include(bp => bp.Category)
                                   .Include(bp => bp.Tag)
                                   .FirstOrDefault(bp => bp.Id == id && bp.Status == "Published");

            if (blogPost == null)
                return NotFound();

            return View(blogPost);
        }



        // Draft
        public IActionResult Draft()
        {
            var draftPosts = _context.BlogPosts
                                     .Where(bp => bp.Status == "Draft" && !bp.IsDeleted)
                                     .Include(bp => bp.Category) 
                                     .Include(bp => bp.Tag)      
                                     .ToList();

            return View(draftPosts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublishDraft(int id)
        {
            var blogPost = _context.BlogPosts.FirstOrDefault(bp => bp.Id == id && bp.Status == "Draft");

            if (blogPost == null)
                return NotFound();

            blogPost.Status = "Published";
            blogPost.IsDraft = false;
            blogPost.IsPublished = true;

            if (blogPost.PublishedDate == null)
            {
                blogPost.PublishedDate = DateTime.Now;
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // Dropdown => draft,publish,archived
        private SelectList GetStatusDropdown()
        {
            var statuses = new[]
            {
                new { Value = "Draft", Text = "Draft" },
                new { Value = "Published", Text = "Published" },
                new { Value = "Archived", Text = "Archived" }
            };

            return new SelectList(statuses, "Value", "Text");
        }
    }
}
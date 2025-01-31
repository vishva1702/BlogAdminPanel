using AutoMapper;
using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogAdminPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public TagController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // List Tags
        public async Task<IActionResult> Index()
        {
            var tags = await _context.Tags
                                .Where(t => !t.IsDeleted)
                                .ToListAsync();

            return View(tags);
        }

        // Create Tag (GET)
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagCreateDto tagDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the form errors and try again.";
                return View(tagDto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not found. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                var tag = _mapper.Map<Tag>(tagDto);
                tag.CreatedBy = userId;
                tag.CreatedOn = DateTime.UtcNow;

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Tag created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(tagDto);
            }
        }

        // Edit Tag (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null || tag.IsDeleted) return NotFound();

            var tagDto = _mapper.Map<TagUpdateDto>(tag);
            return View(tagDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TagUpdateDto tagDto)
        {
            if (!ModelState.IsValid) return View(tagDto);

            var tag = await _context.Tags.FindAsync(tagDto.Id);
            if (tag == null || tag.IsDeleted)
            {
                TempData["ErrorMessage"] = "Tag not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _mapper.Map(tagDto, tag);
                tag.UpdatedBy = userId;
                tag.UpdatedOn = DateTime.UtcNow;

                _context.Update(tag);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Tag updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating tag: {ex.Message}";
                return View(tagDto);
            }
        }

        // Delete Tag (Soft Delete)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null || tag.IsDeleted) return NotFound();

            tag.IsDeleted = true;
            tag.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            tag.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tag deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
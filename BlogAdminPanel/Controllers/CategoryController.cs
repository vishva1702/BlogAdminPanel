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
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoryController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // List Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                                .Where(c => !c.IsDeleted)
                                .ToListAsync();

            return View(categories);
        }

        // Create Category (GET)
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the form errors and try again.";
                return View(categoryDto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    TempData["ErrorMessage"] = "User not found. Please login again.";
                    return RedirectToAction("Login", "Account");
                }

                var category = _mapper.Map<Category>(categoryDto);
                category.CreatedBy = userId;
                category.CreatedOn = DateTime.UtcNow;

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(categoryDto);
            }
        }

        // Edit Category (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null || category.IsDeleted) return NotFound();

            var categoryDto = _mapper.Map<CategoryUpdateDto>(category);
            return View(categoryDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryUpdateDto categoryDto)
        {
            if (!ModelState.IsValid) return View(categoryDto);

            var category = await _context.Categories.FindAsync(categoryDto.Id);
            if (category == null || category.IsDeleted)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _mapper.Map(categoryDto, category);
                category.UpdatedBy = userId;
                category.UpdatedOn = DateTime.UtcNow;

                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating category: {ex.Message}";
                return View(categoryDto);
            }
        }

        // Delete Category (Soft Delete)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null || category.IsDeleted) return NotFound();

            category.IsDeleted = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
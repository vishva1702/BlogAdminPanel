using AutoMapper;
using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlogAdminPanel.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // List Categories
        public IActionResult Index()
        {
            var categories = _context.Categories
                                .Include(c => c.BlogPosts)
                                .Where(c => !c.IsDeleted)
                                .ToList(); // Ensure data is fetched


            return View(categories);
        }


        // Create Category
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryCreateDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var category = _mapper.Map<Category>(categoryDto);
                    category.CreatedBy = "Admin";
                    category.CreatedOn = DateTime.Now;
                    _context.Categories.Add(category);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Category created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving the category: " + ex.Message;
                    TempData["InnerException"] = ex.InnerException?.Message; // Capture inner exception message
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please correct the form errors and try again.";
            }

            return View(categoryDto);
        }

        // Edit Category
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                var categoryDto = _mapper.Map<CategoryUpdateDto>(category);
                return View(categoryDto);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(CategoryUpdateDto categoryupdateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCategory = _context.Categories.Find(categoryupdateDto.Id);
                    if (existingCategory != null)
                    {
                        _mapper.Map(categoryupdateDto, existingCategory);
                        _context.Categories.Update(existingCategory);
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Category not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error occurred while updating the category: " + ex.Message;
            }

            return View(categoryupdateDto);
        }

        //Delete Category
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null || category.IsDeleted)
                return NotFound();

            category.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

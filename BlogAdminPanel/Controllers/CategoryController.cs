using AutoMapper;
using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            var categories = _context.Categories.Include(c => c.BlogPosts).Where(c => !c.IsDeleted).ToList();

            var categoryDtos = _mapper.Map<List<CategoryCreateDto>>(categories);

            return View(categories);  
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateDto CategoryDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var category = _mapper.Map<Category>(CategoryDto);
                    category.CreatedOn = DateTime.Now;

                    _context.Categories.Add(category);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error occurred while creating the user: " + ex.Message;
            }

            return View(CategoryDto);

        }

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

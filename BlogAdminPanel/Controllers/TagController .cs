using AutoMapper;
using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAdminPanel.Controllers
{
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TagController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // List Tags
        public IActionResult Index()
        {
            var tags = _context.Tags
                                .Include(c => c.BlogPosts)
                                .Where(c => !c.IsDeleted)
                                .ToList(); // Ensure data is fetched


            return View(tags);
        }



        // Create Tag
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TagCreateDto tagdto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var tag = _mapper.Map<Tag>(tagdto);
                    tag.CreatedBy = "Admin";
                    tag.CreatedOn = DateTime.Now;
                    _context.Tags.Add(tag);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Category created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while saving the category: " + ex.Message;
                    TempData["InnerException"] = ex.InnerException?.Message; 
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please correct the form errors and try again.";
            }

            return View(tagdto);  
        }


        // Edit Tag
        public IActionResult Edit(int id)
        {
            var tag = _context.Tags.Find(id);
            if (tag != null)
            {
                var tagDto = _mapper.Map<TagUpdateDto>(tag);
                return View(tagDto);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(TagUpdateDto tagupdateDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCategory = _context.Tags.Find(tagupdateDto.Id);
                    if (existingCategory != null)
                    {
                        _mapper.Map(tagupdateDto, existingCategory);
                        _context.Tags.Update(existingCategory);
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

            return View(tagupdateDto);  
        }

        // Delete Tag
        public IActionResult Delete(int id)
        {
            var tag = _context.Tags.Find(id);
            if (tag == null || tag.IsDeleted)
                return NotFound();

            tag.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}

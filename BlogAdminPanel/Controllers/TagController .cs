using AutoMapper;
using BlogAdminPanel.Data;
using BlogAdminPanel.Models;
using BlogAdminPanel.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

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

        // GET: Tag
        public IActionResult Index()
        {
            var tag = _context.Tags.Where(c => !c.IsDeleted).ToList();
            var tagDtos = _mapper.Map<List<TagCreateDto>>(tag);

            return View(tagDtos);
        }
        // GET: Create Tag
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create Tag
        [HttpPost]
        public IActionResult Create(TagCreateDto TagDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var tag = _mapper.Map<Tag>(TagDto);
                    tag.CreatedOn = DateTime.Now;

                    _context.Tags.Add(tag);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error occurred while creating the user: " + ex.Message;
            }

            return View(TagDto);

        }

        // GET: Edit Tag
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

        // POST: Edit Tag
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

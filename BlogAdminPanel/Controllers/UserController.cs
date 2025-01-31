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
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;

        public UserController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<User>();
        }

        public IActionResult Index()
        {
            var users = _context.Users.Where(u => !u.IsDeleted).ToList();
            var userDtos = _mapper.Map<List<UserCreateDto>>(users);
            return View(userDtos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserCreateDto userDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _mapper.Map<User>(userDto);

                    user.PasswordHash = _passwordHasher.HashPassword(user, userDto.PasswordHash);

                    if (userDto.Image != null && userDto.Image.Length > 0)
                    {
                        var imagePath = Path.Combine("wwwroot", "images", userDto.Image.FileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            userDto.Image.CopyTo(stream);
                        }

                        user.Image = $"/images/{userDto.Image.FileName}";
                    }

                    user.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID                    user.CreatedOn = DateTime.Now;

                    // Save to the database
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error occurred while creating the user: " + ex.Message;
            }

            return View(userDto);
        }

        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                var userDto = _mapper.Map<UserUpdateDto>(user);
                return View(userDto);
            }
            return NotFound(); // Return 404 if user is not found
        }

        [HttpPost]
        public IActionResult Edit(UserUpdateDto userDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = _context.Users.Find(userDto.Id);
                    if (existingUser != null)
                    {
                        _mapper.Map(userDto, existingUser);
                        existingUser.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID                        existingUser.UpdatedOn = DateTime.Now;
                        _context.Users.Update(existingUser);
                        _context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "User not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error occurred while updating the user: " + ex.Message;
            }

            return View(userDto);
        }

        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null || user.IsDeleted)
                return NotFound();

            user.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AssignRole(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult AssignRole(int id, string role)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.Role = role;
                user.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID                user.UpdatedOn = DateTime.Now;
                _context.Users.Update(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}

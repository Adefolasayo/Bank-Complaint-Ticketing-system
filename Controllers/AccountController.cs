using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ticketing_system.Models;
using Ticketing_system.Models.DTOs;
using Microsoft.AspNetCore.Identity;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users
    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid login");
            return View();
        }

        if (user == null)
        {
            return Content("User not found");
        }

       

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            return Content("Password failed");
        }

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid login");
            return View();
        }

        // ✅ ONLY HERE: login success

        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserRole", user.Role);
        HttpContext.Session.SetInt32("DepartmentId", user.DepartmentId);
        HttpContext.Session.SetString("FullName", user.FullName);

        if (user.Role == "Agent")
        {
            return RedirectToAction("Index", "Ticket"); // Agent dashboard
        }
        else
        {
            return RedirectToAction("StaffDashboard", "Ticket");
        }


    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var departments = await _context.Departments.ToListAsync();

        var model = new RegisterDto
        {
            AvailableDepartments = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match");
            return View(dto);
        }

       
        var role = dto.DepartmentId == 1 ? "Agent" : "Staff";

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            DepartmentId = dto.DepartmentId,
            Role = role
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");

        
    }

}
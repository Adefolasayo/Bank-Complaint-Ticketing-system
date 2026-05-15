using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketing_system.Models;
using Ticketing_system.Models.Enums;
using Ticketing_system.Service;

public class TicketController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly TicketService _ticketService;

    public TicketController(ApplicationDbContext context, TicketService ticketService)
    {
        _context = context;
        _ticketService = ticketService;
    }

    // 1️⃣ Show form
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TicketCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 🔍 1. Verify Customer Exists
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.AccountNumber == dto.AccountNumber);

        if (customer == null)
        {
            return BadRequest("Invalid account number.");
        }

        // 🔍 2. Verify Name Matches
        if (!customer.FullName.Equals(dto.FullName, StringComparison.OrdinalIgnoreCase))
        {
            // Optional: log fraud attempt here
            return BadRequest("Name does not match account number.");
        }

        // 🧱 3. Create Ticket (WITHOUT TicketCode first)
        var ticket = new Ticket
        {
            Title = dto.Title,
            Description = dto.Description,
            CustomerId = customer.Id,
            Category = dto.Category,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.Now
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // 🎫 4. Generate TicketCode AFTER save
        ticket.TicketCode = _ticketService.GenerateTicketCode(ticket.Id);

        await _context.SaveChangesAsync();

        //        // 🕓 5. Log Ticket History
        //        var history = new TicketHistory
        //        {
        //            TicketId = ticket.Id,
        //            ChangedByUserId = 1, // ⚠️ Replace with logged-in user later
        //            OldStatus = null,
        //            NewStatus = "Open",
        //            ChangedAt = DateTime.Now
        //        };

        //        _context.TicketHistories.Add(history);
        //        await _context.SaveChangesAsync();

        //        // 📧 6. (Optional) Email trigger
        //        // TODO: Send email here

        

        return RedirectToAction("Success", new { code = ticket.TicketCode });

    }

    [HttpGet]
    public async Task<IActionResult> Success(string code)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.TicketCode == code);

        if (ticket == null)
            return NotFound();

        return View(ticket);
    }

    private bool IsAuthorized()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var departmentId = HttpContext.Session.GetInt32("DepartmentId");

        return userId != null;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!IsAuthorized())
            return RedirectToAction("Login", "Account");

        var departmentId = HttpContext.Session.GetInt32("DepartmentId");
        var role = HttpContext.Session.GetString("UserRole");

        IQueryable<Ticket> query = _context.Tickets
            .Include(t => t.Customer)
            .Include(t => t.AssignedToUser)
            .OrderByDescending(t => t.CreatedAt);

        if (role == "Agent")
        {
            // ✅ Agents see ALL tickets
            query = query.OrderByDescending(t => t.CreatedAt);
        }
        else
        {
            // ✅ Staff see ONLY their department tickets
            query = query
                .Where(t => t.DepartmentId == departmentId)
                .OrderByDescending(t => t.CreatedAt);
        }

        var tickets = await query.ToListAsync();

        return View(tickets);
    }

    [HttpGet]
    public async Task<IActionResult> DepartmentTickets()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var departmentId = HttpContext.Session.GetInt32("DepartmentId");
        var fullName = HttpContext.Session.GetString("FullName");
        var role = HttpContext.Session.GetString("UserRole");

        if (userId == null)
            return RedirectToAction("Login", "Account");


        IQueryable<Ticket> query = _context.Tickets
            .Include(t => t.Customer)
            .Include(t => t.AssignedToUser)
            .Where(t => t.DepartmentId == departmentId)
            .OrderByDescending(t => t.CreatedAt);


        var tickets = await query.ToListAsync();

        return View(tickets);
    }



    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (!IsAuthorized())
            return RedirectToAction("Login", "Account");

        var ticket = await _context.Tickets
            .Include(t => t.Customer)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
            return NotFound();

        var departments = await _context.Departments.ToListAsync();

        ViewBag.Departments = departments;

        return View(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, int departmentId)
    {
        if (!IsAuthorized())
            return RedirectToAction("Login", "Account");

        if (HttpContext.Session.GetString("UserRole") != "Agent")
            return Forbid();

        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
            return NotFound();

        ticket.DepartmentId = departmentId;
        ticket.Status = TicketStatus.InProgress;
        ticket.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> StaffDashboard()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var departmentId = HttpContext.Session.GetInt32("DepartmentId");
        var fullName = HttpContext.Session.GetString("FullName");

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var tickets = await _context.Tickets
            .Include(t => t.Customer)
            .Include(t => t.AssignedToUser)
            .Where(t => t.DepartmentId == departmentId
                     && t.AssignedToUserId == null) // 👈 key line
            .ToListAsync();

        var availableTickets = await _context.Tickets
        .Include(t => t.Customer)
        .Include(t => t.AssignedToUser)
        .Where(t => t.DepartmentId == departmentId && t.AssignedToUserId == null)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();

        var myTickets = await _context.Tickets
        .Include(t => t.Customer)
        .Include(t => t.AssignedToUser)
        .Where(t => t.AssignedToUserId == userId && t.Status == TicketStatus.InProgress)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();

        // In Progress
        var inProgressTickets = await _context.Tickets
            .Include(t => t.AssignedToUser)
            .Where(t => t.AssignedToUserId == userId && t.Status == TicketStatus.InProgress)
            .ToListAsync();

        // Resolved
        var resolvedTickets = await _context.Tickets
            .Include(t => t.AssignedToUser)
            .Where(t => t.AssignedToUserId == userId && t.Status == TicketStatus.Resolved)
            .ToListAsync();

        // Closed (optional)
        var closedTickets = await _context.Tickets
            .Include(t => t.AssignedToUser)
            .Where(t => t.AssignedToUserId == userId && t.Status == TicketStatus.Closed)
            .ToListAsync();
        

        var completed = await _context.Tickets
            .Include(t => t.Customer)
            .Include(t => t.AssignedToUser)
            .Where(t =>
                t.AssignedToUserId == userId &&
                (t.Status == TicketStatus.Resolved || t.Status == TicketStatus.Closed))
            .ToListAsync();

        
       
        ViewBag.Completed = completed;

        ViewBag.InProgressTickets = inProgressTickets;
        ViewBag.ResolvedTickets = resolvedTickets;
        ViewBag.ClosedTickets = closedTickets;
        ViewBag.AvailableTickets = availableTickets;
        ViewBag.MyTickets = myTickets;

        return View(tickets);
    }


    [HttpPost]
    public async Task<IActionResult> TakeTicket(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var departmentId = HttpContext.Session.GetInt32("DepartmentId");
        var fullName = HttpContext.Session.GetString("FullName");

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var ticket = await _context.Tickets.FindAsync(id);


        if (ticket.DepartmentId != departmentId)
            return Forbid();

        if (ticket == null)
            return NotFound();

        if (ticket.AssignedToUserId != null)
        {
            return BadRequest("Ticket already taken");
        }

        // Assign ticket to user
        ticket.AssignedToUserId = userId;
        ticket.Status = TicketStatus.InProgress;
        ticket.Status = TicketStatus.InProgress;
        ticket.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Resolve(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var fullName = HttpContext.Session.GetString("FullName");

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
            return NotFound();

        // 🔒 Must be assigned to this user
        if (ticket.AssignedToUserId != userId)
            return Forbid();

        // 🔒 Must be in progress
        if (ticket.Status != TicketStatus.InProgress)
            return BadRequest("Ticket must be in progress before resolving.");

        ticket.Status = TicketStatus.Resolved;
        ticket.ResolvedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction("StaffDashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Close(int id)
    {
        var userRole = HttpContext.Session.GetString("UserRole");

        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
            return NotFound();

        // 🔒 Only allow if resolved first
        if (ticket.Status != TicketStatus.Resolved)
            return BadRequest("Only resolved tickets can be closed.");

        // 🔒 Only agent OR assigned staff
        if (userRole != "Agent" && ticket.AssignedToUserId != HttpContext.Session.GetInt32("UserId"))
            return Forbid();

        ticket.Status = TicketStatus.Closed;
        ticket.ClosedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction("StaffDashboard");
    }
}
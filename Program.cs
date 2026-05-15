using Microsoft.EntityFrameworkCore;
using Ticketing_system.Models;
using Ticketing_system.Service;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddSession();

var app = builder.Build();
app.UseSession();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!context.Departments.Any())
    {
        context.Departments.AddRange(
            new Department { Name = "Customer Support" }, // Id = 1
            new Department { Name = "Technical" },
            new Department { Name = "Fraud & Compliance" },
            new Department { Name = "Account Services" },
            new Department { Name = "Card Services" },
            new Department { Name = "Loans & Credit" },
            new Department { Name = "Billing" }
        );

        context.SaveChanges();
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Ticket/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Ticket}/{action=Create}/{id?}");

app.Run();

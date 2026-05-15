using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Ticketing_system.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<TicketHistory> TicketsHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // UNIQUE Account Number
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.AccountNumber)
            .IsUnique();

        // UNIQUE Ticket Code
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.TicketCode)
            .IsUnique();

        // Relationships (optional but cleaner)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TicketHistory>()
            .HasOne(th => th.ChangedByUser)
            .WithMany()
            .HasForeignKey(th => th.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
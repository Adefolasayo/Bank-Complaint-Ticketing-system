# 🎫 Ticketing System (ASP.NET Core MVC)

A role-based ticketing and complaint management system built with ASP.NET Core MVC and Entity Framework Core.  
This project simulates a real-world support system used in banking or enterprise environments.

---

## 🚀 Features

### 👤 Customer Side
- Submit complaints without login
- Account number + name verification
- Automatic ticket generation
- Email-style confirmation flow (simulated)

---

### 🧑‍💼 Staff Side
- Role-based authentication (Agent / Staff / Admin)
- Department-based ticket visibility
- Staff can:
  - View assigned tickets
  - Accept tickets
  - Resolve tickets
  - Track ticket lifecycle

---

### 🧑‍🔧 Agent Side
- View all tickets across departments
- Assign tickets to departments
- Monitor system-wide activity

---

### 📊 Ticket Lifecycle
- Open → In Progress → Resolved → Closed
- Ticket tracking with timestamps:
  - CreatedAt
  - UpdatedAt
  - ResolvedAt
  - ClosedAt

---

### 📈 Analytics
- Ticket distribution by department
- Resolution time tracking
- complaint analysis

---

## 🏗️ Tech Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQL Server
- C#
- Razor Views
- Custom CSS

---

## 🧠 Key Concepts Implemented

- Role-based access control using Session
- MVC architecture
- DTO pattern for form handling
- Entity Framework relationships
- Ticket workflow state management
- LINQ filtering for dashboards
- Password hashing using ASP.NET Identity PasswordHasher

---

## 🗂️ Database Structure (Simplified)

- Users
- Customers
- Tickets
- Departments
- TicketHistory 
- TicketComments 

---

## 🔐 Authentication

- Secure password hashing
- Session-based login system
- Role + Department-based authorization

---

## ⚙️ How to Run Locally

```bash
git clone https://github.com/your-username/ticketing-system.git
cd ticketing-system

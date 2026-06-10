using face_recognition_api.Models;
using Microsoft.EntityFrameworkCore;

namespace face_recognition_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Credential> Credentials { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<EmployeeRole> EmployeeRoles { get; set; }
    public DbSet<FaceEmbedded> FaceEmbeddeds { get; set; }
    public DbSet<Camera> Cameras { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Employee — manually set PK (EmpId comes from factory system, not auto-generated)
        modelBuilder.Entity<Employee>()
            .HasKey(e => e.EmpId);
        modelBuilder.Entity<Employee>()
            .Property(e => e.EmpId)
            .ValueGeneratedNever();

        // Role — manually set PK
        modelBuilder.Entity<Role>()
            .HasKey(r => r.RoleId);
        modelBuilder.Entity<Role>()
            .Property(r => r.RoleId)
            .ValueGeneratedNever();

        // Camera — manually set PK
        modelBuilder.Entity<Camera>()
            .HasKey(c => c.CameraId);
        modelBuilder.Entity<Camera>()
            .Property(c => c.CameraId)
            .ValueGeneratedNever();

        // EmployeeRole — composite primary key (emp_id, role_id)
        modelBuilder.Entity<EmployeeRole>()
            .HasKey(er => new { er.EmpId, er.RoleId });

        // Credential — one-to-one with Employee
        modelBuilder.Entity<Credential>()
            .HasOne(c => c.Employee)
            .WithOne(e => e.Credential)
            .HasForeignKey<Credential>(c => c.EmpId);

        // FaceEmbedded — many-to-one with Employee (explicit FK)
        modelBuilder.Entity<FaceEmbedded>()
            .HasOne(f => f.Employee)
            .WithMany(e => e.FaceEmbeddeds)
            .HasForeignKey(f => f.EmpId);

        // EmployeeRole — explicit FK to Employee and Role
        modelBuilder.Entity<EmployeeRole>()
            .HasOne(er => er.Employee)
            .WithMany(e => e.EmployeeRoles)
            .HasForeignKey(er => er.EmpId);
        modelBuilder.Entity<EmployeeRole>()
            .HasOne(er => er.Role)
            .WithMany(r => r.EmployeeRoles)
            .HasForeignKey(er => er.RoleId);

        // Transaction — explicit FK + index
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Employee)
            .WithMany(e => e.Transactions)
            .HasForeignKey(t => t.EmpId);
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Camera)
            .WithMany()
            .HasForeignKey(t => t.CameraId);
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.EmpId);
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.CreatedAt);

        // Seed default roles
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "Admin",      IsSystem = true  },
            new Role { RoleId = 2, RoleName = "Supervisor", IsSystem = true  }
        );
    }
}

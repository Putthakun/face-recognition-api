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

        // Transaction — index on emp_id and created_at for fast queries
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.EmpId);
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.CreatedAt);
    }
}

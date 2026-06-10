using System.Text;
using face_recognition_api.Configurations;
using face_recognition_api.Data;
using face_recognition_api.Interfaces;
using face_recognition_api.Models;
using face_recognition_api.Repositories;
using face_recognition_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Bind JwtSettings from appsettings.json ─────────────────────────────
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// ── 2. Register DbContext ──────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── 3. Register Services ───────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();

// ── 3. Configure CORS ─────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:5173" }
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── 4. Configure JWT Authentication ───────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

// ──────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── 4. Database Seed — create first Admin account if none exists ───────────
using (var scope = app.Services.CreateScope())
{
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var hasUsers = await userRepository.AnyAsync();

    if (!hasUsers)
    {
        var adminEmpId = long.Parse(builder.Configuration["AdminSeed:EmpId"] ?? "1111");
        var adminPassword = builder.Configuration["AdminSeed:Password"] ?? "Admin@1234";

        var employee = new Employee { EmpId = adminEmpId, Name = "Admin" };
        var credential = new Credential
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = "Admin"
        };

        await userRepository.CreateAsync(employee, credential);
        Console.WriteLine($"Admin account created. EmpId: {adminEmpId}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");  // ต้องอยู่ก่อน Authentication เสมอ
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

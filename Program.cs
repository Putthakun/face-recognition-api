using System.Text;
using face_recognition_api.Configurations;
using face_recognition_api.Interfaces;
using face_recognition_api.Models;
using face_recognition_api.Repositories;
using face_recognition_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── 1. อ่าน JwtSettings จาก appsettings.json ──────────────────────────────
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// ── 2. ลงทะเบียน Services ─────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();

// ── 3. ตั้งค่า JWT Authentication ─────────────────────────────────────────
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

// ── 4. Database Seed — สร้าง Admin คนแรกถ้ายังไม่มี ───────────────────────
using (var scope = app.Services.CreateScope())
{
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var hasUsers = await userRepository.AnyAsync();

    if (!hasUsers)
    {
        var adminPassword = builder.Configuration["AdminSeed:Password"] ?? "Admin@1234";

        await userRepository.CreateAsync(new User
        {
            Email = builder.Configuration["AdminSeed:Email"] ?? "admin@factory.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = "Admin"
        });

        Console.WriteLine("✅ Admin account created. Email: admin@factory.com");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

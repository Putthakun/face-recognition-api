using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using face_recognition_api.Configurations;
using face_recognition_api.DTOs;
using face_recognition_api.Interfaces;
using face_recognition_api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace face_recognition_api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    // .NET automatically injects IUserRepository and JwtSettings (Dependency Injection)
    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // 1. Find user by employee ID
        var user = await _userRepository.GetByEmpIdAsync(request.EmpId);
        if (user == null) return null;

        // 2. Verify password against stored hash
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid) return null;

        // 3. Generate and return JWT token
        return GenerateToken(user);
    }

    // Called by Admin to create a new employee account
    public async Task<LoginResponseDto> CreateUserAsync(CreateUserDto request)
    {
        // 1. Hash password before storing
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 2. Build user object
        var user = new User
        {
            EmpId = request.EmpId,
            PasswordHash = passwordHash,
            Role = request.Role
        };

        // 3. Persist to repository
        var createdUser = await _userRepository.CreateAsync(user);

        // 4. Return token
        return GenerateToken(createdUser);
    }

    private LoginResponseDto GenerateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours);

        // Claims are embedded in the token so the API knows who the requester is
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("empId", user.EmpId),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}

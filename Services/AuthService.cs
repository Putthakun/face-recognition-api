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

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // 1. Find credential by employee ID (only active accounts)
        var credential = await _userRepository.GetByEmpIdAsync(request.EmpId);
        if (credential == null) return null;

        // 2. Verify password against stored hash
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, credential.PasswordHash);
        if (!isPasswordValid) return null;

        // 3. Generate and return JWT token
        return GenerateToken(credential);
    }

    private LoginResponseDto GenerateToken(Credential credential)
    {
        var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, credential.EmpId.ToString()),
            new Claim("empId", credential.EmpId.ToString()),
            new Claim(ClaimTypes.Role, credential.Role)
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

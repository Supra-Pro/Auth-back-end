using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityAuthLesson.Entities.DTOs;
using IdentityAuthLesson.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityAuthLesson.Services;

public class AuthService : IAuthService
{

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    {
        _configuration = configuration;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<AuthDTO> GenerateToken(User user)
    {
        if (user is null)
        {
            return new AuthDTO()
            {
                Message = "User is null",
                StatusCode = 404
            };
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        
        var key = Encoding.ASCII.GetBytes(_configuration["JWTSettings:SecretKey"]!);

        var roles = await _userManager.GetRolesAsync(user);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(JwtRegisteredClaimNames.NameId, user.Id),
            new(JwtRegisteredClaimNames.Aud, _configuration["JWTSettings:ValidAudience"]!),
            new(JwtRegisteredClaimNames.Iss, _configuration["JWTSettings:ValidIssuer"]!),
            new(JwtRegisteredClaimNames.Exp, _configuration["JwtSettings:ExpireDate"]!),
            new(ClaimTypes.Role, "Admin")
        ];


        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpireDate"])),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthDTO()
        {
            Token = tokenHandler.WriteToken(token),
            Message = "Token created successfully",
            StatusCode = 200,
            IsSuccessful = true
        };
    }
}
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using IdentityAuthLesson.Entities.DTOs;
using IdentityAuthLesson.Entities.Models;
using IdentityAuthLesson.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Full_Stack_Auth.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _inManager;
    private readonly IConfiguration _config;

    public UserController(IAuthService auth, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> inManager, IConfiguration config)
    {
        _auth = auth;
        _userManager = userManager;
        _roleManager = roleManager;
        _inManager = inManager;
        _config = config;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Email(string email)
    {
        var userEmail = email; 
        
        var emailSettings = _config.GetSection("EmailSettings");
        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailSettings["Sender"], emailSettings["SenderName"]),
            Subject = "Reset Password",
            Body = $"Dear {userEmail},\n\n\n\n\nTemporary Code: [8e05511f-6c44-4299-bf6c-cba72556b36a]",
            IsBodyHtml = true,

        };
        mailMessage.To.Add(userEmail);

        using var smtpClient = new SmtpClient(emailSettings["EmailServer"], int.Parse(emailSettings["MailPort"]))
        {
            Port = Convert.ToInt32(emailSettings["MailPort"]),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(emailSettings["Sender"], emailSettings["Password"]),
            EnableSsl = true,
        };
        
        await smtpClient.SendMailAsync(mailMessage);
        
        return Ok("Email sent successfully to: " + userEmail);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new User()
        {
            FullName = registerDto.FullName,
            UserName = registerDto.Email,
            Email = registerDto.Email,
            Status = registerDto.Status,
            Age = registerDto.Age
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        foreach (var role in registerDto.Roles)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return Ok(result);
    }
    
    
    [HttpPost]
    // [HttpOptions]
    [AllowAnonymous]
    public async Task<ActionResult<AuthDTO>> Login(LoginDTO loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null)
        {
            return Unauthorized("User Not Found With This Email");
        }
        
        var test = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!test)
        {
            return Unauthorized("Password invalid");
        }

        var token = await _auth.GenerateToken(user);
        
        return Ok(token);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<string>> GetAllUsers()
    {
        var result = await _userManager.Users.ToListAsync();
        
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
    
    [HttpGet("{id}")]
    [AllowAnonymous] 
    public async Task<ActionResult<User>> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
    
        if (user == null)
        {
            return NotFound(); 
        }

        return Ok(user); 
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(string id, UpdateUserDTO updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.FullName = updateUserDto.FullName;
        user.Age = updateUserDto.Age;
        user.Status = updateUserDto.Status;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(user);
    }
    
}
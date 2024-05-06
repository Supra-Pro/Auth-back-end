using IdentityAuthLesson.Entities.DTOs;
using IdentityAuthLesson.Entities.Models;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Full_Stack_Auth.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    [HttpPost]
    // [AllowAnonymous]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResponseDTO>> CreateRole(RoleDTO role)
    {
        var result = await _roleManager.FindByNameAsync(role.RoleName);

        if (result == null)
        {
            await _roleManager.CreateAsync(new IdentityRole(role.RoleName));
            return Ok(new ResponseDTO
            {
                Message = "Role Created",
                IsSuccessful = true,
                StatusCode = 201 
            });
        }
        return Ok(new ResponseDTO
        {
            Message = "Role Cannot Be Created",
            StatusCode = 403
        });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<IdentityRole>>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.ToListAsync();

        return Ok(roles);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IdentityRole>> GetRoleById(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }
    
}
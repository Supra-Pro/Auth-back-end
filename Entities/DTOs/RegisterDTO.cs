using System.Collections.Generic;

namespace IdentityAuthLesson.Entities.DTOs;

public class RegisterDTO
{
    public string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public int Age { get; set; }
    public string Status { get; set; }
    
    public IList<string> Roles { get; set; }
}
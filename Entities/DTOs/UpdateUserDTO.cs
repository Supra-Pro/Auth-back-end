using System.Collections.Generic;

namespace IdentityAuthLesson.Entities.DTOs;

public class UpdateUserDTO
{
    public string FullName { get; set; }
    public int Age { get; set; }
    public string Status { get; set; }
    public string Email { get; set; } 
}
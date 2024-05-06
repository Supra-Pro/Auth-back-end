using System.Threading.Tasks;
using IdentityAuthLesson.Entities.DTOs;
using IdentityAuthLesson.Entities.Models;

namespace IdentityAuthLesson.Services;

public interface IAuthService
{
    public Task<AuthDTO> GenerateToken(User user);
}
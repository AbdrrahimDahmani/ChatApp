using ChatApp.Entities;

namespace ChatApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser token);
    }
}

using Typefout.Core.Models;

namespace Typefout.Core.Interfaces
{
    public interface IAuthService
    {
        User? Login(string? username, string? email, string password);
    }
}

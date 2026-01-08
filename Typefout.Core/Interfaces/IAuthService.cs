using Typefout.Core.Models;

namespace Typefout.Core.Interfaces
{
    public interface IAuthService
    {
        User CurrentUser { get; set; }
        User? Login(string? username, string? email, string password);
    }
}

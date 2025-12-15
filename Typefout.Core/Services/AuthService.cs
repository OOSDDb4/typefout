using Typefout.Core.Helper;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepo _userRepo;
        public AuthService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public User? Login(string? username, string? email, string password)
        {
            if (username is not null)
            {   
                User? user = _userRepo.GetUser(username);
                if (user is null) return null;
                if (PasswordHelper.VerifyPassword(password, user.Password)) return user;
                return null;
            }
            else if (email is not null)
            {
                User? user = _userRepo.GetMail(email);
                if (user is null) return null;
                if (PasswordHelper.VerifyPassword(password, user.Password)) return user;
                return null;
            }
            else
                return null;
        }
    }
}

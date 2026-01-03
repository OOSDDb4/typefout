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
            User? user = username is not null 
                ? _userRepo.GetUser(username)
                : email is not null 
                    ? _userRepo.GetMail(email)
                    : null;

            if (user is null || !PasswordHelper.VerifyPassword(password, user.Password))
                return null;

            return user;
        }
    }
}
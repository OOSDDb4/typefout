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
        public void Register(string username, string email, string password, string group)
        {
            string hashedPassword = PasswordHelper.HashPassword(password);
            User newUser = new User
            {
                Username = username,
                Email = email,
                Password = hashedPassword,
                Group = group
            };
            _userRepo.AddUser(newUser);
        }
    }
}

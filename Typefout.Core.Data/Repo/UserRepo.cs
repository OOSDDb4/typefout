using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Google.GenAI;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Repo
{
    public class UserRepo : IUserRepo
    {
        private readonly List<User> _usersList;

        public UserRepo()
        {
            _usersList = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@mail.com", Password = "FLeqWLF6n2rMxgeYy5aAgQ==.drm7OYTXB2dsptZcCjxje3W0Z8yqDOx5qhqXaZ4t4bs=", Group = "None" }
            };
        }

        public List<User> GetAllUsers()
        {
            return _usersList;
        }

        public User? GetMail(string mail)
        {
            User? user = _usersList.FirstOrDefault(c => c.Email.Equals(mail));
            return user;
        }

        public User? GetUser(string username)
        {
            User? user = _usersList.FirstOrDefault(c => c.Username == username);
            return user;
        }

        public User? GetUserById(int id)
        {
            User? user = _usersList.FirstOrDefault(c => c.Id == id);
            return user;
        }

        public void AddUser(User user)
        {
            user.Id = _usersList.Count > 0 ? _usersList.Max(u => u.Id) + 1 : 1;
            _usersList.Add(user);
        }

    }
}

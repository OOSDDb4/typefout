using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typefout.Core.Models;

namespace Typefout.Core.Interfaces
{
    public interface IUserRepo
    {
        public User? GetUser(string username);
        public User? GetUserById(int id);
        public User? GetMail(string mail);

        public List<User> GetAllUsers();
        public void AddUser(User user);
    }
}

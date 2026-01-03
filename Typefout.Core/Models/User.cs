using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Typefout.Core.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        public string Group { get; set; }

        public User()
        {
        }

        public User(int id, string username, string email, string password, string group)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            Group = group;
        }
    }
}

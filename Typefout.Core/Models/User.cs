using Newtonsoft.Json;

namespace Typefout.Core.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public UserType UserType { get; set; }

        public int SchoolId { get; set; }

        public int? GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

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
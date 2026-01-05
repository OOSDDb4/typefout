using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Repo
{
    public class UserRepo : IUserRepo
    {
        private readonly string _filePath;

        public UserRepo()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.CreateDirectory(basePath);

            _filePath = Path.Combine(basePath, "users.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }

            SeedDefaultUser();
        }
        //auth
        public User? GetUser(string username)
        {
            List<User> users = Load();
            return users.FirstOrDefault(u => u.Username == username);
        }

        public User? GetMail(string mail)
        {
            List<User> users = Load();
            return users.FirstOrDefault(u => u.Email == mail);
        }

        public User? GetUserById(int id)
        {
            List<User> users = Load();
            return users.FirstOrDefault(u => u.Id == id);
        }
        //admin
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            List<User> users = await LoadAsync();
            return users;
        }

        public async Task<IEnumerable<User>> GetTeachersBySchoolIdAsync(int schoolId)
        {
            List<User> users = await LoadAsync();
            return users.Where(u =>
                u.UserType == UserType.Docent &&
                u.SchoolId == schoolId);
        }

        public async Task<IEnumerable<User>> GetStudentsByGroupIdAsync(int groupId)
        {
            List<User> users = await LoadAsync();
            return users.Where(u =>
                u.UserType == UserType.Leerling &&
                u.GroupId == groupId);
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            List<User> users = await LoadAsync();
            return users.FirstOrDefault(u => u.Id == userId);
        }

        public async Task SetActiveAsync(int userId, bool isActive)
        {
            List<User> users = await LoadAsync();
            User? existing = users.FirstOrDefault(u => u.Id == userId);
            if (existing == null) return;

            existing.IsActive = isActive;

            await SaveAsync(users);
        }
        public async Task CreateAsync(User user)
        {
            List<User> users = await LoadAsync();

            int nextId = users.Count == 0
                ? 1
                : users.Max(u => u.Id) + 1;

            user.Id = nextId;
            users.Add(user);

            await SaveAsync(users);
        }

        public async Task UpdateAsync(User user)
        {
            List<User> users = await LoadAsync();
            User? existing = users.FirstOrDefault(u => u.Id == user.Id);
            if (existing == null) return;

            existing.Username = user.Username;
            existing.Email = user.Email;
            existing.Password = user.Password;
            existing.UserType = user.UserType;
            existing.SchoolId = user.SchoolId;
            existing.GroupId = user.GroupId;

            await SaveAsync(users);
        }

        public async Task DeleteAsync(int userId)
        {
            List<User> users = await LoadAsync();

            User? user = users.FirstOrDefault(s => s.Id == userId);
            if (user != null)
            {
                users.Remove(user);
                await SaveAsync(users);
            }
        }
        //users
        private List<User> Load()
        {
            string json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<User>();
            }

            List<User>? users = JsonSerializer.Deserialize<List<User>>(json);
            return users ?? new List<User>();
        }

        private async Task<List<User>> LoadAsync()
        {
            string json = await File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<User>();
            }

            List<User>? users = JsonSerializer.Deserialize<List<User>>(json);
            return users ?? new List<User>();
        }

        private async Task SaveAsync(List<User> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_filePath, json);
        }

        private void SeedDefaultUser()
        {
            List<User> users = Load();
            if (users.Count > 0) return;

            users.Add(new User
            {
                Id = 1,
                Username = "user1",
                Email = "user1@mail.com",
                Password = "FLeqWLF6n2rMxgeYy5aAgQ==.drm7OYTXB2dsptZcCjxje3W0Z8yqDOx5qhqXaZ4t4bs=",
                UserType = UserType.Admin,
                SchoolId = 1
            });

            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }
    }
}

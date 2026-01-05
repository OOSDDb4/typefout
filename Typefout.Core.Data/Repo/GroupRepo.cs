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
    public class GroupRepo : IGroupRepo
    {
        private readonly string _filePath;

        public GroupRepo()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.CreateDirectory(basePath);

            _filePath = Path.Combine(basePath, "groups.json");

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            List<Group> groups = await LoadAsync();
            return groups;
        }
        public async Task<IEnumerable<Group>> GetBySchoolIdAsync(int schoolId)
        {
            List<Group> groups = await LoadAsync();
            return groups.Where(g => g.SchoolId == schoolId);
        }

        public async Task<Group?> GetByIdAsync(int groupId)
        {
            List<Group> groups = await LoadAsync();
            return groups.FirstOrDefault(g => g.Id == groupId);
        }

        public async Task CreateAsync(Group group)
        {
            List<Group> groups = await LoadAsync();

            int nextId = groups.Count == 0
                ? 1
                : groups.Max(g => g.Id) + 1;

            group.Id = nextId;
            groups.Add(group);

            await SaveAsync(groups);
        }

        public async Task UpdateAsync(Group group)
        {
            List<Group> groups = await LoadAsync();

            Group? existing = groups.FirstOrDefault(g => g.Id == group.Id);
            if (existing == null)
                return;

            existing.Name = group.Name;
            existing.TeacherId = group.TeacherId;
            existing.TeacherName = group.TeacherName;

            await SaveAsync(groups);
        }

        public async Task DeleteAsync(int groupId)
        {
            List<Group> groups = await LoadAsync();

            Group? group = groups.FirstOrDefault(s => s.Id == groupId);
            if (group != null)
            {
                groups.Remove(group);
                await SaveAsync(groups);
            }
        }

        private async Task<List<Group>> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return new List<Group>();

            string json = File.ReadAllTextAsync(_filePath).GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(json))
                return new List<Group>();

            List<Group>? groups = JsonSerializer.Deserialize<List<Group>>(json);
            return groups ?? new List<Group>();
        }

        private async Task SaveAsync(List<Group> groups)
        {
            string json = JsonSerializer.Serialize(groups, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}

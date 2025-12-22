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
    public class SchoolRepo : ISchoolRepo
    {
        private readonly string _filePath;

        public SchoolRepo()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.CreateDirectory(basePath);
            _filePath = Path.Combine(basePath, "schools.json");
          
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        public async Task<IEnumerable<School>> GetAllAsync()
        {
            List<School> schools = await LoadAsync();
            return schools;
        }

        public async Task<School?> GetByIdAsync(int schoolId)
        {
            List<School> schools = await LoadAsync();
            School? school = schools.FirstOrDefault(s => s.Id == schoolId);
            return school;
        }

        public async Task CreateAsync(School school)
        {
            List<School> schools = await LoadAsync();

            int nextId = schools.Count == 0
                ? 1
                : schools.Max(s => s.Id) + 1;

            school.Id = nextId;
            schools.Add(school);

            await SaveAsync(schools);
        }

        public async Task UpdateAsync(School school)
        {
            List<School> schools = await LoadAsync();

            School? existing = schools.FirstOrDefault(s => s.Id == school.Id);
            if (existing != null)
            {
                existing.Name = school.Name;
                await SaveAsync(schools);
            }
        }

        public async Task DeleteAsync(int schoolId)
        {
            List<School> schools = await LoadAsync();

            School? school = schools.FirstOrDefault(s => s.Id == schoolId);
            if (school != null)
            {
                schools.Remove(school);
                await SaveAsync(schools);
            }
        }

        private async Task<List<School>> LoadAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<School>();
            }

            string json = await File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<School>();
            }

            List<School>? schools = JsonSerializer.Deserialize<List<School>>(json);

            return schools ?? new List<School>();
        }


        private async Task SaveAsync(List<School> schools)
        {
            string json = JsonSerializer.Serialize(schools, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}

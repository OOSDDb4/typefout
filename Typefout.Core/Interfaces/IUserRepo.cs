using System.Collections.Generic;
using System.Threading.Tasks;
using Typefout.Core.Models;

namespace Typefout.Core.Interfaces
{
    public interface IUserRepo
    {
        User? GetUser(string username);
        User? GetMail(string mail);
        User? GetUserById(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<IEnumerable<User>> GetTeachersBySchoolIdAsync(int schoolId);
        Task<IEnumerable<User>> GetStudentsByGroupIdAsync(int groupId);
        Task<IEnumerable<User>> GetStudentsBySchoolIdAsync(int schoolId);
        Task SetActiveAsync(int userId, bool isActive);

        public List<User> GetAllUsers();
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int userId);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Typefout.Core.Models;

namespace Typefout.Core.Interfaces;

public interface IGroupRepo
{
    Task<IEnumerable<Group>> GetAllAsync();
    Task<IEnumerable<Group>> GetBySchoolIdAsync(int schoolId);
    Task<Group?> GetByIdAsync(int groupId);
    Task CreateAsync(Group group);

    Task UpdateAsync(Group group);

    Task DeleteAsync(int groupId);
}
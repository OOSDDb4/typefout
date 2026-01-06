using System.Collections.Generic;
using System.Threading.Tasks;
using Typefout.Core.Models;

namespace Typefout.Core.Interfaces;

public interface ISchoolRepo
{
    Task<IEnumerable<School>> GetAllAsync();

    Task<School?> GetByIdAsync(int schoolId);

    Task CreateAsync(School school);

    Task UpdateAsync(School school);

    Task DeleteAsync(int schoolId);
    Task DeleteCascadeAsync(int schoolId);

}
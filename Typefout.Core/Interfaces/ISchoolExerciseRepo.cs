using Typefout.Core.Models;

namespace Typefout.Core.Interfaces;

public interface ISchoolExerciseRepo
{
    Task<List<SchoolExerciseStatus>> GetStatusBySchoolAsync(int schoolId);
    Task LinkAsync(int schoolId, int exerciseId, bool schoolActive = true);
    Task UnlinkAsync(int schoolId, int exerciseId);
    Task SetSchoolActiveAsync(int schoolId, int exerciseId, bool active);
    Task<List<Exercise>> GetAvailableExercisesForSchoolAsync(int schoolId);
}
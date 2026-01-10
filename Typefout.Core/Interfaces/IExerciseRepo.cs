using Typefout.Core.Models;

namespace Typefout.Core.Interfaces;

public interface IExerciseRepo
{
    Task<List<Exercise>> GetAllAsync();
    Task SetGlobalActiveAsync(int exerciseId, bool active);
}
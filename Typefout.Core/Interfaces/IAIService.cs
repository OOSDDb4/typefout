using System.Threading.Tasks;
using Typefout.Core.Models;

namespace Typefout.Core.Interfaces
{
    public interface IAiService
    {
        Task<TypingExerciseText> GetExerciseTextAsync(string mode);
    }
}
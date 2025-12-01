using Typefout.Core.Models;

namespace Typefout.Core.Interfaces
{
    public interface ISentenceService
    {
        OefeningZin GetRandomized();
        List<OefeningZin> GetAllSentences();
    }
}
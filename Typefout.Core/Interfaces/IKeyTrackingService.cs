using System.Collections.Generic;
using Typefout.Core.Models;

namespace Typefout.Core.Interfaces
{
    public interface IKeyTrackingService
    {
        int TotalAttempts { get; }
        int TotalMistakes { get; }
        void RegisterResult(char expected, char typed);
        IReadOnlyList<KeyStat> GetStats();
        IReadOnlyList<char> GetMostDifficultKeys(int count);
        void Reset();
    }
}
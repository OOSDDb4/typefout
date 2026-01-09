using System.Collections.Generic;
using System.Linq;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Services
{
    public class KeyTrackingService : IKeyTrackingService
    {
        private readonly Dictionary<char, (int attempts, int mistakes)> _stats =
            new Dictionary<char, (int attempts, int mistakes)>();

        public int TotalAttempts { get; private set; }
        public int TotalMistakes { get; private set; }

        public void RegisterResult(char expected, char typed)
        {
            if (expected == '?' || expected == '\0')
                return;

            if (expected == ' ')
                expected = '_';

            TotalAttempts++;

            (int attempts, int mistakes) data =
                _stats.ContainsKey(expected) ? _stats[expected] : (0, 0);

            data.attempts++;

            if (typed != expected)
            {
                data.mistakes++;
                TotalMistakes++;
            }

            _stats[expected] = data;
        }
        public IReadOnlyList<KeyStat> GetStats()
        {
            return _stats
                .Where(kvp => kvp.Key != '_')     // 🚫 Hide spacebar
                .Select(kvp => new KeyStat
                {
                    Key = kvp.Key,
                    Attempts = kvp.Value.attempts,
                    Mistakes = kvp.Value.mistakes
                })
                .OrderByDescending(s => s.Attempts)
                .ToList();
        }
        public IReadOnlyList<char> GetMostDifficultKeys(int count)
        {
            return _stats
                .Where(kvp => kvp.Key != '_')  // hide space
                .Select(kvp => new KeyStat
                {
                    Key = kvp.Key,
                    Attempts = kvp.Value.attempts,
                    Mistakes = kvp.Value.mistakes
                })
                .Where(s => s.Attempts >= 5) // meaningful keys only
                .OrderByDescending(s => s.ErrorRate)
                .Take(count)
                .Select(s => s.Key)
                .ToList();
        }

        public void Reset() => _stats.Clear();
    }
}

using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Services
{
    public class SentenceService : ISentenceService
    {
        private readonly List<string> _sentences = new()
        {
            "Dit is een voorbeeldzin.",
            "Windesheim is een mooie school.",
            "Programmeren is soms lastig maar leuk.",
            "Deze oefening helpt je sneller typen.",
            "Vandaag leren we nieuwe dingen."
        };

        private readonly Random _random = new();

        public OefeningZin GetRandomized()
        {
            string text = _sentences[_random.Next(_sentences.Count)];
            return new OefeningZin { Text = text };
        }

        public List<OefeningZin> GetAllSentences()
        {
            return _sentences
                .Select(s => new OefeningZin { Text = s })
                .ToList();
        }
    }
}
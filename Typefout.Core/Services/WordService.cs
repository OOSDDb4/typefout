using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Services
{
    public class WordService : IWordService
    {
        private readonly List<string> _words = new()
        {"Windesheim","Testen","Project"};

        private readonly Random _random = new();

        public OefenWoord GetRandomized()
        {
            string wordText = _words[_random.Next(_words.Count)];
            return new OefenWoord { Text = wordText };
        }

    }
}

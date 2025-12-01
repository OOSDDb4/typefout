using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Services
{
    public class AIService : IAIService
    {
        private readonly string _apiKey;
        private readonly Client _client;

        public AIService()
        {
            EnvService.Load();

            _apiKey = EnvService.Get("API_KEY")
                ?? throw new InvalidOperationException("API_KEY not found in environment.");
            _client = new Client(apiKey: _apiKey);
        }


        private readonly List<string> _recentSentences = new List<string>();
        private const int _maxHistorySize = 10;

        private readonly Schema _exerciseSchema = new Schema
        {
            Properties = new Dictionary<string, Schema> {
                { "exercise_text", new Schema { Type = Google.GenAI.Types.Type.STRING,
                    Description = "The generated sentence in Dutch, semantically correct." } },
            },
            PropertyOrdering = new List<string> { "exercise_text" },
            Required = new List<string> { "exercise_text" },
            Title = "TypingExercise",
            Type = Google.GenAI.Types.Type.OBJECT
        };

        private readonly List<char> _errorkeys = new List<char> { };

        private string BuildPrompt()
        {
            string avoidList = _recentSentences.Count > 0
                ? string.Join("; ", _recentSentences)
                : "None";

            return $@"
            ROLE: You are a typing tutor engine.
            TASK: Generate a practice sentence based on provided ERROR_KEYS.
        
            INPUT DATA:
            - ERROR_KEYS: [{string.Join(", ", _errorkeys)}]
            - PREVIOUSLY_GENERATED (MUST AVOID): [{avoidList}]

            CONSTRAINTS:
            1. Output MUST be valid JSON.
            2. Language: Dutch.
            3. Content: Neutral, safe for all ages. No violent or political themes.
            4. Density: At least 20% of the characters must be from ERROR_KEYS.
            5. Grammar: Syntactically correct sentences, valid Dutch words.
            6. UNIQUENESS: The new sentence MUST be completely different from the sentences in PREVIOUSLY_GENERATED. Do not recycle the same subject or structure.
            7. Size: Sentence can be max 8 words in length.
            8. Level: the level is for a 20 year old";
        }

        public async Task<TypingExerciseText> GetExerciseTextAsync()
        {

            GenerateContentResponse response = await _client.Models.GenerateContentAsync(
                model: "gemini-2.0-flash",
                contents: BuildPrompt(),
                config: new GenerateContentConfig
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = _exerciseSchema,
                    Temperature = 0.75f
                });

            string jsonResponse = response.Candidates[0].Content.Parts[0].Text;

            TypingExerciseText? exerciseData = JsonSerializer.Deserialize<TypingExerciseText>(jsonResponse);
            if (exerciseData != null && !string.IsNullOrWhiteSpace(exerciseData.Text))
            {
                UpdateHistory(exerciseData.Text);
            }

            return new TypingExerciseText { Text = exerciseData.Text };
        }

        private void UpdateHistory(string newSentence)
        {
            _recentSentences.Add(newSentence);
            if (_recentSentences.Count > _maxHistorySize)
            {
                _recentSentences.RemoveAt(0);
            }
        }
    }
}

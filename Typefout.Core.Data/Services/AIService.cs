using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Services
{
    public class AiService : IAiService
    {
        private readonly string _apiKey;
        private readonly Client _client;
        private readonly IKeyTrackingService _tracking;

        public AiService(IKeyTrackingService tracking)
        {
            _tracking = tracking;

            EnvService.Load();

            _apiKey = EnvService.Get("API_KEY")
                ?? throw new InvalidOperationException("API_KEY not found in environment.");

            _client = new Client(apiKey: _apiKey);
        }

        private readonly List<string> _recentHistory = new List<string>();
        private const int _maxHistorySize = 15;

        private Schema BuildSchema()
        {
            return new Schema
            {
                Properties = new Dictionary<string, Schema> {
                    { "exercise_text", new Schema {
                        Type = Google.GenAI.Types.Type.STRING,
                        Description = "The generated Dutch word or sentence." }
                    },
                },
                PropertyOrdering = new List<string> { "exercise_text" },
                Required = new List<string> { "exercise_text" },
                Title = "TypingExercise",
                Type = Google.GenAI.Types.Type.OBJECT
            };
        }

        private string BuildPrompt(string mode)
        {
            IReadOnlyList<char> difficultKeys = _tracking.GetMostDifficultKeys(5);
            string difficultList = difficultKeys.Count > 0
                ? string.Join(", ", difficultKeys)
                : "None";

            string avoidList = _recentHistory.Count > 0
                ? string.Join("; ", _recentHistory)
                : "None";

            if (mode == "word")
            {
                return $@"
ROLE: You are a typing practice generator.

TASK: Generate ONE Dutch word.

CONSTRAINTS:
1. Output MUST be valid JSON.
2. Key: ""exercise_text"" = the generated word.
3. Avoid previously generated words: [{avoidList}]
4. TRY TO INCLUDE DIFFICULT KEYS: [{difficultList}]
5. Word must be 3–12 characters.
6. Only one word. No sentences.
7. No names or offensive content.";
            }

            return $@"
ROLE: You are a typing tutor engine.

TASK: Generate ONE Dutch sentence.

CONSTRAINTS:
1. Output MUST be valid JSON.
2. Key: ""exercise_text"" = the generated sentence.
3. Avoid previously generated sentences: [{avoidList}]
4. TRY TO INCLUDE DIFFICULT KEYS: [{difficultList}]
5. Length: 5–10 words.
6. Grammar: Correct Dutch.
7. Safe for all ages.
8. UNIQUE and different from [{avoidList}].";
        }

        public async Task<TypingExerciseText> GetExerciseTextAsync(string mode)
        {
            Schema schema = BuildSchema();
            string prompt = BuildPrompt(mode);

            GenerateContentResponse response = await _client.Models.GenerateContentAsync(
                model: "gemini-2.0-flash",
                contents: prompt,
                config: new GenerateContentConfig
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = schema,
                    Temperature = 0.7f
                });

            string jsonResponse = response.Candidates[0].Content.Parts[0].Text;

            TypingExerciseText? result =
                JsonSerializer.Deserialize<TypingExerciseText>(jsonResponse);

            if (result != null && !string.IsNullOrWhiteSpace(result.Text))
            {
                UpdateHistory(result.Text);
            }

            return new TypingExerciseText
            {
                Text = result?.Text ?? "fallback"
            };
        }

        private void UpdateHistory(string text)
        {
            _recentHistory.Add(text);

            if (_recentHistory.Count > _maxHistorySize)
            {
                _recentHistory.RemoveAt(0);
            }
        }
    }
}

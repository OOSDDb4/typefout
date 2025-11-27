using System.Text.Json; // Ensure this is present for JsonSerializer
using Google.GenAI;
using Google.GenAI.Types;
using Typefout.Core.Interfaces;
using Typefout.Core.Models;

namespace Typefout.Core.Data.Services
{
    public class AIService : IAIService
    {
        // Fix IDE1006: Add _ prefix for private fields
        private readonly Client _client = new Client(apiKey: "AIzaSyC0_iPqt4lVpifIiwe-5lS6Tr1WVrf03uY");

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

        private readonly List<char> _errorkeys = new List<char> { 'A', 'E', 'T', 'H', 'B' };

        private string Prompt => $@"
        ROLE: You are a typing tutor engine.
        TASK: Generate a practice sentence based on provided ERROR_KEYS.
        INPUT: ERROR_KEYS = [{string.Join(", ", _errorkeys)}]
        CONSTRAINTS:
        1. Output MUST be valid JSON.
        2. Language: Dutch.
        3. Content: Neutral, safe for all ages. No violent or political themes.
        4. Density: At least 20% of the characters must be from ERROR_KEYS.
        5. Grammar: Syntactically correct sentences, no random string of words, only use valid Dutch woorden ";

        public async Task<TypingExerciseText> GetExerciseTextAsync()
        {
            // Fix CS0103: Use _client instead of client
            // Fix CS1992: Method is already async
            // Fix CS0825: 'var' is valid here as a local variable declaration

            GenerateContentResponse response = await _client.Models.GenerateContentAsync(
                model: "gemini-2.0-flash",
                contents: Prompt,
                config: new GenerateContentConfig
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = _exerciseSchema,
                    Temperature = 0.5f
                });

            string jsonResponse = response.Candidates[0].Content.Parts[0].Text;

            TypingExerciseText? exerciseData = JsonSerializer.Deserialize<TypingExerciseText>(jsonResponse);
            return new TypingExerciseText { Text = exerciseData.Text };
        }
    }
}

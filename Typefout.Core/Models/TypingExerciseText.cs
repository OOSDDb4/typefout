using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Typefout.Core.Models
{
    public class TypingExerciseText
    {
        [JsonPropertyName("exercise_text")]
        public required string Text { get; set; }
    }
}

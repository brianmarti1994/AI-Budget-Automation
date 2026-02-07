using Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.AI
{
    public sealed class OllamaAiCommandParser : IAiCommandParser
    {
        private readonly OllamaClient _client;
        private readonly OllamaOptions _opt;

        public OllamaAiCommandParser(OllamaClient client, OllamaOptions opt)
        {
            _client = client;
            _opt = opt;
        }

        public async Task<ParsedAiCommand> ParseAsync(string userPrompt, int currentYear, CancellationToken ct)
        {
            try
            {

           
            var system = """
You are a strict JSON API. Output ONLY valid JSON. No markdown, no commentary.
Task: parse a user's budgeting prompt into an intent + fields.

Allowed intent values:
- CreateCategory
- UpdateBudget
- DeleteCategory
- Unknown

Rules:
- Extract category name if present.
- Extract year if present; else null.
- Extract total amount if present; else null.
- If user specifies months as a list for allocation, return Months=[...] (1..12).
- If user specifies explicit month amounts (e.g. "March to 200"), return MonthBudgets object: { "3": 200 }.
- For DeleteCategory: set RequiresConfirmation=true and a ConfirmationMessage.

Output schema:
{
  "Intent": "CreateCategory|UpdateBudget|DeleteCategory|Unknown",
  "CategoryName": "string|null",
  "Year": number|null,
  "TotalAmount": number|null,
  "Months": [number]|null,
  "MonthBudgets": { "1": number, "2": number }|null,
  "RequiresConfirmation": true|false,
  "ConfirmationMessage": "string|null"
}

Month mapping: Jan=1 ... Dec=12.
If prompt has "current year", use null for Year (server will default).
""";

            var user = $"""
CurrentYear={currentYear}
Prompt={userPrompt}
""";

            var raw = await _client.ChatAsync(_opt.Model, system, user, ct);

            // Defensive parse: if model returns extra text, try to extract first JSON object.
            var json = ExtractJsonObject(raw);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
    {
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    }
                };

                var parsed = JsonSerializer.Deserialize<ParsedAiCommand>(json, options);
            return parsed ?? new ParsedAiCommand { Intent = AiIntent.Unknown };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private static string ExtractJsonObject(string text)
        {
            var start = text.IndexOf('{');
            var end = text.LastIndexOf('}');
            if (start < 0 || end < 0 || end <= start) return "{}";
            return text.Substring(start, end - start + 1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AI
{
    public sealed class OllamaClient
    {
        private readonly HttpClient _http;

        public OllamaClient(HttpClient http) => _http = http;

        public async Task<string> ChatAsync(string model, string system, string user, CancellationToken ct)
        {
            // Ollama /api/chat
            var payload = new
            {
                model,
                stream = false,
                messages = new[]
                {
                new { role = "system", content = system },
                new { role = "user", content = user }
            }
            };

            var resp = await _http.PostAsJsonAsync("/api/chat", payload, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: ct);
            return json?.message?.content ?? "";
        }

        private sealed class OllamaChatResponse
        {
            public Message? message { get; set; }
            public sealed class Message
            {
                public string? role { get; set; }
                public string? content { get; set; }
            }
        }
    }
}

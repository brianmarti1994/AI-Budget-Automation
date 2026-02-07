using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AI
{
    public sealed class OllamaOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:11434";
        public string Model { get; set; } = "gpt-oss:120b-cloud";
    }
}

using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ErrorMessages : IErrorMessages
    {
        private readonly Dictionary<string, Dictionary<string, string>> _messages;

        public ErrorMessages()
        {
            // путь до JSON файла (относительно запускаемого проекта)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resources", "Errors.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Не найден файл локализации: {filePath}");

            var json = File.ReadAllText(filePath);
            _messages = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json)
                        ?? new();
        }

        public string GetMessage(string key, string lang, params object[] args)
        {
            if (_messages.TryGetValue(lang, out var dict) &&
                dict.TryGetValue(key, out var template))
            {
                return string.Format(template, args);
            }

            return key; // fallback
        }

    }
}

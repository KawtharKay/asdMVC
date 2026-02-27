using Application.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;

        public GeminiService(
            HttpClient httpClient,
            IOptions<GeminiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GetResponseAsync(
            string userMessage,
            List<ChatHistoryItem> history)
        {
            var contents = new List<object>();

            // System prompt — tells Gemini its role
            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = "You are a helpful and friendly customer support agent for an ecommerce store. Only answer questions related to orders, products, wallet funding, and general support. Keep responses short and helpful." } }
            });

            contents.Add(new
            {
                role = "model",
                parts = new[] { new { text = "Hello! I am your support assistant. How can I help you today?" } }
            });

            // Add conversation history for context
            foreach (var item in history)
            {
                contents.Add(new
                {
                    role = item.Role,
                    parts = new[] { new { text = item.Content } }
                });
            }

            // Add current message
            contents.Add(new
            {
                role = "user",
                parts = new[] { new { text = userMessage } }
            });

            var payload = new { contents };

            var response = await _httpClient.PostAsync(
                $"{_settings.BaseUrl}?key={_settings.ApiKey}",
                new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            return json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?
                .Value<string>()
                ?? "Sorry I could not process your request right now. Please try again.";
        }
    }
}

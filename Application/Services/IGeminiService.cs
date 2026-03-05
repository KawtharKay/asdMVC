namespace Application.Services
{
    public interface IGeminiService
    {
        Task<string> GetResponseAsync(
            string userMessage,
            List<ChatHistoryItem> history);
    }

    public class ChatHistoryItem
    {
        public string Role { get; set; } = default!;  // "user" or "model"
        public string Content { get; set; } = default!;
    }
}

namespace DevnotChatbot.API.Services
{
    public interface IDevnotChatbotService
    {
        Task<List<string>> InterpretDevNot(string userDream, string apiKey, string filePath);
    }
}

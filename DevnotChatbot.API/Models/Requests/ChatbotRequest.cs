using System.ComponentModel.DataAnnotations;

namespace DevnotChatbot.API.Models.Requests
{
    public class ChatbotRequest
    {
        [MinLength(1, ErrorMessage = "Request cannot be empty.")]
        public required string Request { get; set; }
    }
}

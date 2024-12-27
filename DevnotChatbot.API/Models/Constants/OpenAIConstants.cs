using System.ComponentModel.DataAnnotations;

namespace DevnotChatbot.API.Models.Constants
{
    public class OpenAIConstants
    {
        public const string Key = "OpenAI";

        [Required(ErrorMessage = "ApiKey is required.")]
        public string ApiKey { get; set; } = default!;

        [Required(ErrorMessage = "FilePath is required.")]
        public string FilePath { get; set; } = default!;
    }
}

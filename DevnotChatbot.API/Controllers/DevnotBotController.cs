using DevnotChatbot.API.Models.Constants;
using DevnotChatbot.API.Models.Requests;
using DevnotChatbot.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace DevnotChatbot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevnotBotController : ControllerBase
    {
        private readonly IDevnotChatbotService _devnotChatbotService;
        private readonly OpenAIConstants _openAIConstants;
        public DevnotBotController(IConfiguration configuration, IOptions<OpenAIConstants> openAIConstants, IDevnotChatbotService devnotChatbotService)
        {
            // API Key ve dosya yolunu appsettings.json'dan Option Pattern ile birlikte program.cs içerisinde doğrulayıp alıyoruz
            _openAIConstants = openAIConstants.Value;
            // DreamInterpretationService'i başlat
            _devnotChatbotService = devnotChatbotService;
        }
        [HttpPost("interpret")]
        public async Task<IActionResult> InterpretChatbot([FromBody] ChatbotRequest devnotRequest)
        {
            var interpretation = await _devnotChatbotService.InterpretDevNot(devnotRequest.Request, _openAIConstants.ApiKey, _openAIConstants.FilePath);
            return Ok(interpretation);
        }
    }


}

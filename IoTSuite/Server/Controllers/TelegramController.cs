using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using IoTSuite.Server.Extensions;
using IoTSuite.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace IoTSuite.Server.Controllers
{
    [ApiController]
    [NewtonsoftJsonFormatter]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(AuthenticationSchemes = "Basic")]
    public class TelegramController : ControllerBase
    {
        private readonly ITelegramWebhookService _telegramWebhookService;

        public TelegramController(ITelegramWebhookService telegramWebhookService)
        {
            _telegramWebhookService = telegramWebhookService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            try
            {
                await _telegramWebhookService.EchoAsync(update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return Ok();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace IoTSuite.Server.Services
{
    public interface ITelegramWebhookService
    {
        Task EchoAsync(Update update);
    }
}

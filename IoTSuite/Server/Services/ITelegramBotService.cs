using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace IoTSuite.Server.Services
{
    public interface ITelegramBotService
    {
        TelegramBotClient Client { get; }

        int ChatId { get; }
    }
}

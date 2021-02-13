using IoTSuite.Server.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace IoTSuite.Server.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        public TelegramBotService(IOptions<TelegramBotConfiguration> config)
        {
            Client = new TelegramBotClient(config.Value.BotToken);

            ChatId = config.Value.ChatId;
        }

        public TelegramBotClient Client { get; }

        public int ChatId { get; }
    }
}

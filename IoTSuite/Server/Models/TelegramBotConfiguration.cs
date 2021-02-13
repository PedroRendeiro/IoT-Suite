using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTSuite.Server.Models
{
    public class TelegramBotConfiguration
    {
        public string BotToken { get; set; }

        public string Socks5Host { get; set; }

        public int Socks5Port { get; set; }

        public int ChatId { get; set; }
    }
}

using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT
{
    public class Bot
    {
        private const string _defaultBotName = "Bot";
        private DiscordSocketClient _discordClient = null;

        public Bot(string token, string name = null)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Name = string.IsNullOrEmpty(name) ? _defaultBotName : name;
            Token = token;
        }

        public string Name
        {
            get;
        }

        private string Token
        {
            get;
        }
    }
}

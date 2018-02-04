using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBOT
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            string token = BotConfiguration.Token;
            Bot bot = new Bot(token);
            bot.LoginAndStart();

            await Task.Delay(-1);
        }

    }
}

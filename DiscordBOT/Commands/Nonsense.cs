using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBOT.Commands
{
    public class Nonsense: ModuleBase<SocketCommandContext>
    {
        [Command("nonsense")]
        [Summary("Echos nonsense.")]
        public async Task SayNonsense()
        {
            List<string> nonsense = BotConfiguration.Nonsense;
            Random random = new Random();

            int randInt = random.Next(0, nonsense.Count - 1);
            string response = nonsense[randInt];

            await ReplyAsync(response);

        }
    }
}

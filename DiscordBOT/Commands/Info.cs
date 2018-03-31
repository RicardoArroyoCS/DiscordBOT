using Discord.Commands;
using DiscordBOT.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBOT.Commands
{
    public class Info: SocketCommandContextModuleBase
    {

        [Command("say")]
        [Summary("Echos a message.")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            if (IsUserTheClient)
            {
                return;
            }
            await ReplyAsync(echo);
        }
    }
}

using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBOT.Commands
{
    public class Info: ModuleBase<SocketCommandContext>
    {

        [Command("say")]
        [Summary("Echos a message.")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            await ReplyAsync(echo);
        }
    }
}

using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Utils
{
    public class SocketCommandContextModuleBase: ModuleBase<SocketCommandContext>
    {
        public async void ReplyAsyncError(string message)
        {
            if(!string.IsNullOrEmpty(message))
            {
                await ReplyAsync($"[Invalid Operation]:{message}");
            }
        }
    }
}

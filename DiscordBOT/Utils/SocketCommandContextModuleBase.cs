using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Utils
{
    public class SocketCommandContextModuleBase: ModuleBase<SocketCommandContext>
    {
        public bool IsUserTheClient
        {
            get
            {
                return this.Context.User.Get_DiscriminatorInt()
                    == this.Context.Client.CurrentUser.Get_DiscriminatorInt();
            }
        }

        public async void ReplyAsyncError(string message)
        {
            if(!string.IsNullOrEmpty(message))
            {
                await ReplyAsync($"[Invalid Operation]:{message}");
            }
        }

        public async void ReplyAsyncCodeBlock(string message)
        {
            if(!string.IsNullOrEmpty(message))
            {
                await ReplyAsync($"```{message}```");
            }
        }
    }
}

using Discord;
using Discord.Commands;
using DiscordBOT.DataAccess;
using DiscordBOT.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBOT.Commands
{
    public class Status: SocketCommandContextModuleBase
    {
        [Command("status")]
        [Summary("Posts the status message assigned by the user")]
        public async Task StatusAsync()
        {
            IUser user = this.Context.User;
            int userId = user.Get_DiscriminatorInt();
            string statusMessage = ReputationDataAccess.Instance.GetUserStatusMessage(userId);

            if(string.IsNullOrEmpty(statusMessage))
            {
                ReplyAsyncCodeBlock($"[{user.Username}] does not have a status message set up.");
                return;
            }

            await ReplyAsync(statusMessage);
        }
    }
}

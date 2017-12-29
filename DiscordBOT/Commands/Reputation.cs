using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBOT.Utils;

namespace DiscordBOT.Commands
{
    public class Reputation: SocketCommandContextModuleBase
        //ModuleBase<SocketCommandContext>
    {
        [Command("rep")]
        [Summary("Gives Reputation to the user specified")]
        public async Task RepAsync([Summary("The user rep is being given to")] IUser user, string value)
        {
            if(ValidateReputationRequest(user, value, out int repModifierValue))
            {
                await ReplyAsync($"Reputation successfuly added: {repModifierValue}");
            }
        }

        private bool ValidateReputationRequest(IUser user, string repModifier, out int repModifierValue)
        {
            bool isValid = false;
            repModifierValue = 0;

            if (this.Context.User.Id == user.Id)
            {
                ReplyAsyncError("The user cannot give reputation to his/herself.");
            }
            else
            {
                if (user.TryParseReputation(repModifier, out repModifierValue))
                {
                    isValid = true;
                }
                else
                {
                    ReplyAsyncError($"value '{repModifier}' is invalid. Use '+' or '-'");
                }
            }

            return isValid;
        }
    }
}

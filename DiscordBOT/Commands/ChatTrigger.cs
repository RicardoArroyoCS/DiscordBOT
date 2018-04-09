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
    class ChatTrigger: SocketCommandContextModuleBase
    {
        [Command("tregister")]
        [Summary("Registers the specified trigger")]
        public async Task RegisterAsync(string triggerName, string triggerValue)
        {
            IUser user = Context.User;
            int chatId = Context.Guild.Get_ID();
            int userId = user.Get_DiscriminatorInt();
            ChatTriggerRequest request = new ChatTriggerRequest(chatId, userId, triggerName, triggerValue);

            bool success = ChatDataAccess.Instance.CreateChatTrigger(request);

            if(success)
            {
                ReplyAsyncCodeBlock($"Successfully created trigger for [{triggerName}]");
            }
            else
            {
                ReplyAsyncCodeBlock($"Failed to create trigger for [{triggerName}]");
            }
        }

        [Command(">")]
        [Summary("Gets the value of a specified trigger name")]
        public async Task GetTriggerValue([Remainder]string triggerName)
        {
            int chatId = Context.Guild.Get_ID();

            string triggerValue = ChatDataAccess.Instance.GetTriggerValue(chatId, triggerName);
            if (!string.IsNullOrEmpty(triggerValue))
            {
                Embed embed = null;
                if(Uri.TryCreate(triggerValue, UriKind.Absolute, out _))
                {
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                    {
                        ImageUrl = triggerValue
                    };
                    embed = embedBuilder.Build();
                }
                await ReplyAsync(triggerValue, embed:embed);
            }
            else
            {
                ReplyAsyncCodeBlock($"Could not find value for trigger [{triggerName}]");
            }
        }
    }
}

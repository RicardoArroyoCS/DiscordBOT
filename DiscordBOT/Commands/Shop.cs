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
    public class Shop: SocketCommandContextModuleBase
    {
        [Command("shop")]
        [Summary("Lists available shop commands while displaying a user's spendable rep")]
        public async Task ShopAsync()
        {
            await ReplyAsync(ShopManager.DisplayShopItems());
        }

        [Command("shop")]
        [Summary("Fulfills the shop request given the user has enough reputation")]
        public async Task ShopAsync(string request, params string[] requestParams)
        {
            ShopItem shopItem = ShopManager.GetShopItem(request);
            IUser user = Context.User;
            string userName = user.Get_UserHandleName();
            int userId = user.Get_DiscriminatorInt();
            IUser mod =  this.Context.Channel.GetUserAsync(BotConfiguration.ModId).Result;

            if (shopItem == null)
            {
                ReplyAsyncCodeBlock($"Shop item {request} does not exist");
                return;
            }

            if (!ShopManager.ValidateArguments(shopItem, requestParams))
            {
                ReplyAsyncCodeBlock($"Request format incorrect. Please provide sufficient parameters ie !shop {shopItem.Name} <param1> <param2>");
                return;
            }

            int availableRep = ReputationDataAccess.GetAvailableReputationToSpend(userId);

            if (availableRep < shopItem.Cost)
            {
                ReplyAsyncCodeBlock($"[{userName}] has [{availableRep}] rep. [{shopItem.Cost}] is required for this item");
                return;
            }

            bool isSuccessfulUpdate = ReputationDataAccess.UpdateAvailableReputationToSpend(userId, shopItem.Cost);

            if(!isSuccessfulUpdate)
            {
                ReplyAsyncError("An error occured attempting to update reputation spent. Transaction was not complete");
                return;
            }
            availableRep = ReputationDataAccess.GetAvailableReputationToSpend(userId);
            ReplyAsyncCodeBlock($"[{userName}] has purchased [{shopItem.Name}] for [{shopItem.Cost}]. Remaining balance: [{availableRep}]");

            if(!shopItem.IsManualRequest)
            {
                bool isSucess = ShopManager.PreformAutomaticRequest(user, shopItem, this.Context, requestParams);

                if(isSucess)
                {
                    ReplyAsyncCodeBlock($"Request for shop item [{shopItem.Name}] has successfully been applied");
                }
                else
                {
                    await ReplyAsync($"[{mod.Mention}] An error occured attempting to apply shop item [{shopItem.Name}]");
                }
            }
            else
            {
                await ReplyAsync($"{mod.Mention} request for [{userName}] to have purchased shop item [{shopItem.Name}] applied");
            }

        }        

    }
}

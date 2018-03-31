using Discord;
using Discord.Commands;
using DiscordBOT.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordBOT.Utils
{
    public static class ShopManager
    {
        private const string _chatStatusItemName = "ChatStatus";
        private const string _botImgItemName = "BotImg";
        private const string _emoticonItemName = "Emoticon";
        private const string _statusItemName = "Status";
        private const string roleItemName = "Role";
        private const string _fanclubItemName = "FanClub";
        public const string _tableFormat = "{0, -12}|{1, -5}|{2, -25}|{3, -10}";
        static ShopManager()
        {
            InitializeShopItems();
        }

        public static ShopItems Items
        {
            get;
            set;
        } = new ShopItems();

        private static void InitializeShopItems()
        {
            //Items.Add(new ShopItem("ChatStatus", 15, "Change chat status", false, 1));
            Items.Add(new ShopItem("BotImg", 50, "Change the bot image", true));
            Items.Add(new ShopItem("Emoticon", 75, "Add a custom emoticon", true));
            Items.Add(new ShopItem("Status", 150, "Change your user status", false, 1));
            Items.Add(new ShopItem("Role", 500, "Create a custom role", true));
            Items.Add(new ShopItem("FanClub", 1000, "Create a custom fan club", true));
        }

        public static ShopItem GetShopItem(string itemName)
        {
            return Items.Where(i => itemName.Equals(i.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        public static string DisplayShopItems()
        {
            string columnHeaders = String.Format($"```{_tableFormat}\n", "Name", "Cost", "Description", "Type");
            StringBuilder stringBuilder = new StringBuilder(columnHeaders);
            foreach(ShopItem item in Items)
            {
                stringBuilder.Append($"{item.ToString()}\n\n");
            }
            //stringBuilder.Append($"This feature is not fully implemented. None of the shop items are purchaseable at this moment.{Environment.NewLine}");
            stringBuilder.Append("*Contact Admin to perform manual requests. Automated requests will be performed by bot```");
            stringBuilder.Append($"*This feature is still in beta.*{Environment.NewLine}");
            return stringBuilder.ToString();
        }

        public static bool ValidateArguments(ShopItem item, params string[] argument)
        {
            return (item.NumberParams <= argument.Count());
        }

        public static bool CheckUserSufficientRep(int userId, int repWorth)
        {
            int repToSpend = ReputationDataAccess.GetAvailableReputationToSpend(userId);

            return repToSpend >= repWorth;
        }

        public static bool PreformAutomaticRequest(IUser user, ShopItem item, SocketCommandContext context, params string[] requestParam)
        {
            int userId = user.Get_DiscriminatorInt();            
            switch (item.Name)
            {
                case _chatStatusItemName:
                    UInt64 chatId = context.Guild.Id;
                    return ReputationDataAccess.UpdateChatStatus(chatId, string.Join(' ', requestParam));
                case _statusItemName:
                    return ReputationDataAccess.UpdateUserMessage(userId, string.Join(' ', requestParam));
                default:
                    Console.WriteLine($"ShopItem was not found: {item.Name}");
                    break;
            }
            return false;
        }
    }
}

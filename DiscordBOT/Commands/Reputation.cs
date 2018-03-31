using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBOT.Utils;
using DiscordBOT.DataAccess;

namespace DiscordBOT.Commands
{
    public class Reputation: SocketCommandContextModuleBase
        //ModuleBase<SocketCommandContext>
    {
        [Command("rep")]
        [Summary("Returns the Rep value associated with a user")]
        public async Task RepAsync()
        {
            ReplyUserReputation(Context.User);
        }

        [Command("rep")]
        [Summary("Returns the Rep value associated with specified user")]
        public async Task RepAsync(IUser user)
        {
            ReplyUserReputation(user);
        }

        [Command("repboard")]
        [Summary("Posts the top 5 users with the most REP")]
        public async Task RepBoardAsync()
        {
            RepBoardAsync(5);
        }

        [Command("repboard")]
        [Summary("Posts the top number count of users with the most REP")]
        public async Task RepBoardAsync(int count)
        {
            DiscordUsers discordUsers = ReputationDataAccess.GetRepBoard(count);
            string tableFormat = "{0,-32}|{1,-12}";
            StringBuilder stringBuilder = new StringBuilder(String.Format($"{tableFormat}{Environment.NewLine}", "UserName", "Reputation"));

            foreach(DiscordUser user in discordUsers)
            {
                stringBuilder.Append(String.Format($"{tableFormat}{Environment.NewLine}", user.UserName, user.ReputationValue));
            }
            ReplyAsyncCodeBlock(stringBuilder.ToString());
        }

        [Command("rep")]
        [Summary("Gives Reputation to the user specified")]
        public async Task RepAsync([Summary("The user rep is being given to")] IUser user, string value)
        {
            IUser fromUser = Context.User;
            IUser toUser = user;
            int fromUserId = fromUser.Get_DiscriminatorInt();
            if (IsUserTheClient)
            {
                return;
            }
            if (ValidateReputationRequest(user, value, out int repModifierValue))
            {
                if (ReputationDataAccess.CheckOrCreateUserRecord(toUser) && 
                    ReputationDataAccess.CheckOrCreateUserRecord(fromUser))
                {
                    ReputationDataAccess.UpdateAvailableReputation(fromUserId);
                    bool isAvailableToRep = ReputationDataAccess.IsUserAbleToRep(fromUserId, repModifierValue);

                    if(isAvailableToRep)
                    {
                        if (ReputationDataAccess.AddReputation(toUser.Get_DiscriminatorInt(), fromUser.Get_DiscriminatorInt(), repModifierValue))
                        {
                            int? userAvailableRep = ReputationDataAccess.GetUserReputationAvailability(fromUserId);                            
                            string addedRepMessage = $"[{toUser.Username}]: +[{repModifierValue}] REP";
                            if (userAvailableRep.HasValue)
                            {
                                ReplyAsyncCodeBlock($"{addedRepMessage}. [{fromUser.Username}] Distributable REP: [{userAvailableRep.Value}].");
                            }
                            else
                            {
                                ReplyAsyncCodeBlock($"{addedRepMessage}. An error occured trying to retrieve [{fromUser.Username}]'s available REP.");
                            }
                        }
                        else
                        {
                            ReplyAsyncError($"Error Adding reputation to [{user.Username}]");
                        }
                    }
                    else
                    {
                        ReplyAsyncCodeBlock($"User [{fromUser.Username}] has not accumulated enough rep to give.");
                    }
                }
                else
                {
                    ReplyErrorCreatingUserRecord();
                }
            }
        }

        private async void ReplyUserReputation(IUser user)
        {
            int userId = user.Get_DiscriminatorInt();
            string userName = user.Username;

            if (ReputationDataAccess.CheckOrCreateUserRecord(user))
            {
                ReputationDataAccess.UpdateAvailableReputation(userId);
                string reputation = ReputationDataAccess.GetUserReputation(userId);

                if (!string.IsNullOrEmpty(reputation))
                {
                    int? userAvailableRep = ReputationDataAccess.GetUserReputationAvailability(userId);
                    if (userAvailableRep.HasValue)
                    {
                        ReplyAsyncCodeBlock($"[{userName}]: User REP: [{reputation}]; Distributable REP: [{userAvailableRep.Value}].");
                    }
                    else
                    {
                        ReplyAsyncCodeBlock($"[{userName}]: User REP: [{reputation}]; An error occured trying to retrieve distributable REP.");
                    }
                }
                else
                {
                    ReplyAsyncError($"Error Retrieving {userName}'s REP.");
                }
            }
            else
            {
                ReplyErrorCreatingUserRecord();
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

        private void ReplyErrorCreatingUserRecord()
        {
            ReplyAsyncError("Error Creating New User in Database.");
        }
    }
}

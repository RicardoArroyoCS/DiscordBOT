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
            IUser user = Context.User;
            int userId = user.Get_DiscriminatorInt();
            string userName = Context.User.Username;

            if (ReputationDataAccess.CheckOrCreateUserRecord(user))
            {
                ReputationDataAccess.UpdateAvailableReputation(userId);
                string reputation = ReputationDataAccess.GetUserReputation(userId);

                if (!string.IsNullOrEmpty(reputation))
                {
                    int? userAvailableRep = ReputationDataAccess.GetUserReputationAvailability(userId);
                    if(userAvailableRep.HasValue)
                    {
                        await ReplyAsync($"{userName} currently has '{reputation}' REP with '{userAvailableRep.Value}' available REP to distribute.");            
                    }
                    else
                    {
                        await ReplyAsync($"{userName} currently has '{reputation}' REP. An error occured trying to retrieve available REP.");
                    }
                }
                else
                {
                    ReplyAsyncError($"Error Retrieving {userName}'s REP");
                }
            }
            else
            {
                ReplyErrorCreatingUserRecord();
            }

        }

        [Command("rep")]
        [Summary("Gives Reputation to the user specified")]
        public async Task RepAsync([Summary("The user rep is being given to")] IUser user, string value)
        {
            IUser fromUser = Context.User;
            IUser toUser = user;
            int fromUserId = fromUser.Get_DiscriminatorInt();

            if (ValidateReputationRequest(user, value, out int repModifierValue))
            {
                if (ReputationDataAccess.CheckOrCreateUserRecord(toUser) && 
                    ReputationDataAccess.CheckOrCreateUserRecord(fromUser))
                {
                    ReputationDataAccess.UpdateAvailableReputation(fromUserId);
                    bool isAvailableToRep = ReputationDataAccess.IsUserAbleToRep(fromUserId);

                    if(isAvailableToRep)
                    {
                        if (ReputationDataAccess.AddReputation(toUser.Get_DiscriminatorInt(), fromUser.Get_DiscriminatorInt(), repModifierValue))
                        {
                            int? userAvailableRep = ReputationDataAccess.GetUserReputationAvailability(fromUserId);
                            string addedRepMessage = $"Reputation successfully added: {repModifierValue}.";
                            if (userAvailableRep.HasValue)
                            {
                                await ReplyAsync($"{addedRepMessage} {fromUser.Username} has '{userAvailableRep.Value}' available REP to distribute.");
                            }
                            else
                            {
                                await ReplyAsync($"{addedRepMessage} An error occured trying to retrieve {fromUser.Username}'s available REP.");
                            }
                        }
                        else
                        {
                            ReplyAsyncError($"Error Adding reputation to {user.Username}");
                        }
                    }
                    else
                    {
                        await ReplyAsync($"User {fromUser.Username} has not accumulated enough rep to give.");
                    }
                }
                else
                {
                    ReplyErrorCreatingUserRecord();
                }
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

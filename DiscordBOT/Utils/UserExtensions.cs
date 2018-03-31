using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Utils
{
    public static class UserExtensions
    {

        #region Extension Properties
        public static int Get_Reputation(this IUser user)
        {
            return 0;
        }

        public static bool Set_Reputation(this IUser user)
        {
            bool isSet = false;

            return isSet;
        }

        public static string Get_UserHandleName(this IUser user)
        {
            return $"@{user.Username}";
        }

        public static int Get_DiscriminatorInt(this IUser user)
        {
            return (int)user.DiscriminatorValue;
        }
        #endregion

        #region Extension Methods
        public static bool TryParseReputation(this IUser user, string value, out int output)
        {
            output = 0;

            switch(value.ToLower())
            {
                case "+":
                    output = 1;
                    break;
                case "plus":
                    output = 1;
                    break;
                case "-":
                    output = -1;
                    break;
                case "minus":
                    output = -1;
                    break;
            }
            if(output == 0 && int.TryParse(value, out int reputationValue))
            {
                output = reputationValue;
            }

            return output != 0;
        }
        #endregion
    }
}

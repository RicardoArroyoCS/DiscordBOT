using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Utils
{
    public static class UserExtensions
    {
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

            return output != 0;
        }
    }
}

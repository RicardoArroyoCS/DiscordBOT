using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Utils
{
    public static class GuildExtensions
    {
        #region Extension Properties
        public static int Get_ID(this SocketGuild guild)
        {
            return (int)guild.Id;
        }
        #endregion
    }
}

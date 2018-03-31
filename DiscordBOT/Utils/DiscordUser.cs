using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DiscordBOT.Utils
{
    public class DiscordUser
    {
        public DiscordUser(string userName, string reputationValue)
        {
            UserName = userName;
            ReputationValue = reputationValue;
        }

        public DiscordUser(SqlDataReader reader)
        {
            UserName = reader["UserName"]?.ToString() ?? "Unknown User";
            ReputationValue = reader["TotalReputation"]?.ToString() ?? "0";
        }

        public string UserName
        {
            get;
            set;
        }

        public string ReputationValue
        {
            get;
            set;
        }
    }
}

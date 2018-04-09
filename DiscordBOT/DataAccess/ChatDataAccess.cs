using DiscordBOT.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DiscordBOT.DataAccess
{
    public class ChatDataAccess: DataAccess
    {
        private static readonly Lazy<ChatDataAccess> _lazy
            = new Lazy<ChatDataAccess>(() => new ChatDataAccess());

        public static ChatDataAccess Instance
        {
            get
            {
                return _lazy.Value;
            }
        }

        #region Set Data Access Methods
        public bool CreateChatSetting(int chatId, string chatName)
        {
            return PerformSqlInsertOrUpdate(
                (SqlConnection connection) => 
                {
                    string query = "MERGE " +
                                    "[ReputationBot].[dbo].[Chat_Settings] AS T " +
                                    "USING (SELECT @chatID AS ID, @chatName as SNAME) AS S " +
                                    "	ON (T.ChatID = S.ID) " +
                                    "WHEN NOT MATCHED BY TARGET " +
                                    "	THEN INSERT (ChatID, ChatName) VALUES (S.ID, S.SNAME)" +
                                    "WHEN MATCHED " +
                                    "	THEN UPDATE SET T.ChatName = S.SNAME;";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@chatID", chatId);
                    command.Parameters.AddWithValue("@chatName", chatName);
                    return command;
                });
        }


        public bool CreateChatTrigger(ChatTriggerRequest request)
        {
            return PerformSqlInsertOrUpdate(
                (SqlConnection connection) =>
                {
                    string query =  "MERGE " +
                                    "[ReputationBot].[dbo].[Chat_Triggers] AS T " +
                                    "USING (SELECT @chatID AS chatID, @userID as userID, @triggerName as tName, @triggerType as tType, @triggerValue as tValue) AS S " +
                                    "	ON (T.ChatID = S.chatID AND T.TriggerName = S.tName) " +
                                    "WHEN NOT MATCHED BY TARGET " +
                                    "	THEN INSERT (ChatID, UserID, TriggerName, TriggerType, TriggerValue) VALUES (S.chatID, S.userID, S.tName, S.tType, s.tValue) " +
                                    "WHEN MATCHED " +
                                    "	THEN UPDATE SET T.UserID = S.userID, T.TriggerValue = s.tValue;";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@chatID", request.ChatID);
                    command.Parameters.AddWithValue("@userID", request.UserID);
                    command.Parameters.AddWithValue("@triggerName", request.TriggerName);
                    command.Parameters.AddWithValue("@triggerType", request.TriggerType);
                    command.Parameters.AddWithValue("@triggerValue", request.TriggerValue);
                    return command;
                });

        }
        #endregion

        #region Get Data Access Methods
        public string GetTriggerValue(int chatId, string triggerName)
        {
            string triggerValue = PerformSqlSingleRead(
                (SqlConnection connection) =>
                {
                    string query =  "SELECT TriggerValue " +
                                    "FROM [ReputationBot].[dbo].[Chat_Triggers] " +
                                    "WHERE ChatID = @chatID AND TriggerName = @triggerName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@chatID", chatId);
                    command.Parameters.AddWithValue("@triggerName", triggerName);
                    return command;
                },
                (SqlDataReader reader) =>
                {
                    string val = "cannot find triggervalue";
                    while(reader.Read())
                    {
                        return reader["TriggerValue"].ToString();
                    }
                    return val;
                });

            if(string.IsNullOrEmpty(triggerValue))
            {
                triggerValue = null;
            }
            return triggerValue;
        }

        #endregion
    }
}

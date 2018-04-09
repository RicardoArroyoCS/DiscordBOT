using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Utils
{
    public class ChatTriggerRequest
    {
        public ChatTriggerRequest(int chatId, int userId, string triggerName, string triggerValue, string triggerType)
            :this(chatId, userId, triggerName, triggerValue)
        {
            TriggerType = TriggerType;
        }

        public ChatTriggerRequest(int chatId, int userId, string triggerName, string triggerValue)
        {
            ChatID = chatId;
            UserID = userId;
            TriggerName = triggerName;
            TriggerValue = triggerValue;
            TriggerType = string.Empty;
        }

        public int ChatID
        {
            get;
            set;
        }

        public int UserID
        {
            get;
            set;
        }

        public string TriggerName
        {
            get;
            set;
        }

        public string TriggerType
        {
            get;
            set;
        }

        public string TriggerValue
        {
            get;
            set;
        }
    }
}

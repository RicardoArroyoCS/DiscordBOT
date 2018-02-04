using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Utils
{
    public class AddReputationResponse
    {
        public AddReputationResponse(bool isSuccess, int fromUserAvailableRep)
        {
            IsSuccess = isSuccess;
            FromUserAvailableReputation = fromUserAvailableRep;
        }

        public bool IsSuccess
        {
            get;
            set;
        }

        public int FromUserAvailableReputation
        {
            get;
            set;
        }
    }
}

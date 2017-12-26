using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBOT.Exceptions
{
    public class ConfigurationSettingNotFound: Exception
    {
        public ConfigurationSettingNotFound(string message)
            :base(message)
        {
        }
    }
}

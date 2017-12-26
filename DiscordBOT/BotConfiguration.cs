using DiscordBOT.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBOT
{
    // Schema From:
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?tabs=basicconfiguration
    // Extension Methods Added from https://stackoverflow.com/questions/36001695/setting-base-path-using-configurationbuilder
    public class BotConfiguration
    {
        private static IConfigurationBuilder _builder = null;

        public static IConfigurationRoot Configuration
        {
            get
            {
                if(_builder == null)
                {
                    try
                    {
                        _builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
                    }
                    catch(Exception)
                    {
                        throw new Exception("appsettings.json File Not Found");
                    }
                }

                return _builder.Build();
            }
        }

        public static string Token
        {
            get
            {
                string token = string.Empty;

                try
                {
                    token = Configuration["bot:token"];
                }
                catch(Exception)
                {
                    throw new ConfigurationSettingNotFound("bot:token not found in configuration file");
                }

                return token;
            }
        }

    }
}

using DiscordBOT.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiscordBOT
{
    // Schema From:
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?tabs=basicconfiguration
    // Extension Methods Added from https://stackoverflow.com/questions/36001695/setting-base-path-using-configurationbuilder
    public class BotConfiguration
    {
        private static IConfigurationBuilder _builder = null;
        private static string _token = null;
        private static List<string> _nonsense = null;
        private static string _dbConnectionString = null;

        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_builder == null)
                {
                    try
                    {
                        _builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
                    }
                    catch (Exception)
                    {
                        throw new Exception("appsettings.json File Not Found");
                    }
                }

                return _builder.Build();
            }
        }

        public static string DatabaseConnectionString
        {
            get
            {
                if(_dbConnectionString == null)
                {
                    try
                    {
                        _dbConnectionString = Configuration["connectionString"];
                    }
                    catch (Exception)
                    {
                        throw new ConfigurationSettingNotFound("bot:token not found in configuration file");
                    }
                }

                return _dbConnectionString;
            }
        }


        public static string Token
        {
            get
            {
                if(_token == null)
                {
                    try
                    {
                        _token = Configuration["bot:token"];
                    }
                    catch(Exception)
                    {
                        throw new ConfigurationSettingNotFound("bot:token not found in configuration file");
                    }
                }

                return _token;
            }
        }

        public static List<string> Nonsense
        {
            get
            {
                if(_nonsense == null)
                {
                    try
                    {
                        _nonsense = Configuration.GetSection("nonsense").AsEnumerable().Where(r => !string.IsNullOrEmpty(r.Value)).Select(r => r.Value).ToList();
                    }
                    catch(Exception ex)
                    {
                        throw new ConfigurationSettingNotFound("nonsense not found in configuration file. Error message:"+ ex.Message);
                    }
                }
                return _nonsense;
            }
        }

    }
}

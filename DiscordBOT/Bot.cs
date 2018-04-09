using Discord.Commands;
using Discord.WebSocket;
using DiscordBOT.Commands;
using DiscordBOT.DataAccess;
using DiscordBOT.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBOT
{
    public class Bot
    {
        private const string _defaultBotName = "Bot";

        public Bot(string token, string name = null)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Name = string.IsNullOrEmpty(name) ? _defaultBotName : name;
            Token = token;
            DiscordClient = new DiscordSocketClient();
            Commands = new CommandService();

            InitializeServicesAndEvents();
        }

        #region Properties
        public string Name
        {
            get;
        }

        private string Token
        {
            get;
        }

        public DiscordSocketClient DiscordClient
        {
            get;
        }

        private IServiceProvider Services
        {
            get;
            set;
        }

        private CommandService Commands
        {
            get;
        }
        #endregion

        #region Initialize
        private void InitializeServicesAndEvents()
        {
            Services = new ServiceCollection()
                        .AddSingleton(DiscordClient)
                        .AddSingleton(Commands)
                        .BuildServiceProvider();

            Commands.AddModuleAsync<Info>();
            Commands.AddModuleAsync<Nonsense>();
            Commands.AddModuleAsync<Reputation>();
            Commands.AddModuleAsync<Shop>();
            Commands.AddModuleAsync<Status>();
            Commands.AddModuleAsync<ChatTrigger>();

            DiscordClient.MessageReceived += DiscordClient_MessageReceived;
            DiscordClient.Ready += DiscordClient_Ready;
        }

        private Task DiscordClient_Ready()
        {
            foreach (SocketGuild guild in DiscordClient.Guilds)
            {                
                ChatDataAccess.Instance.CreateChatSetting(guild.Get_ID(), guild.Name);
            }
            return null;
        }

        private async Task DiscordClient_MessageReceived(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            bool isTrigger = false;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            //if (!( (message.HasCharPrefix('!', ref argPos) || message.HasCharPrefix('>', ref argPos)) || message.HasMentionPrefix(DiscordClient.CurrentUser, ref argPos))) return;
            //if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(DiscordClient.CurrentUser, ref argPos))) return;
            if (!(message.HasCharPrefix('!', ref argPos) || (isTrigger = message.HasCharPrefix('>', ref argPos)) || message.HasMentionPrefix(DiscordClient.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new SocketCommandContext(DiscordClient, message);

            // if Trigger, get triggervalue
            if (isTrigger)
            {
                argPos = 0;
            }
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await Commands.ExecuteAsync(context, argPos, Services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        #endregion

        #region Login
        public async void LoginAndStart()
        {
            await Login();
            await Start();
        }

        private async Task Login()
        {
            Task result = null;
            try
            {
                result = DiscordClient.LoginAsync(Discord.TokenType.Bot, Token);
                Task.WaitAll(result);
            }
            catch(Exception ex)
            {
                throw new Exception("Invalid Token", ex);
            }
        }

        private async Task Start()
        {
            Task result = null;
            try
            {
                result =  DiscordClient.StartAsync();
                Task.WaitAll(result);
            }
            catch(Exception ex)
            {
                throw new Exception("Error Attempting to start Discord Client", ex);
            }
        }

        #endregion
    }
}

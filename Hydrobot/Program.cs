using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;


namespace Hydrobot {
    class Program {

        private DiscordSocketClient client;
        private CommandService commands;

        static void Main(string[] args) {
            new Program().MainAsync().GetAwaiter().GetResult();
        }
        private async Task MainAsync() {
            client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Debug
            });

            commands = new CommandService(new CommandServiceConfig {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            client.MessageReceived += Client_MessageRecieved;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            client.Ready += Client_Ready;
            client.Log += Client_Log;

            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("hydrationToken"));
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Log(LogMessage message) {
            Console.WriteLine($"{DateTime.Now} at {message.Source} {message.Message}");

        }

        private async Task Client_Ready() {
            await client.SetGameAsync("this is the yeetest!");
        }

        private async Task Client_MessageRecieved(SocketMessage messageParam) {
            var message = messageParam as SocketUserMessage;
            var context = new SocketCommandContext(client, message);
            
            if (context.Message == null || context.Message.Content == "") {
                return;
            }

            if (context.User.IsBot) {
                return;
            }

            int argPos = 0;
            if (!(message.HasStringPrefix("!", ref argPos)) || message.HasMentionPrefix(client.CurrentUser, ref argPos)) {
                return;
            }

            var result = await commands.ExecuteAsync(context, argPos, null);
            if (!result.IsSuccess) {
                Console.WriteLine($"{DateTime.Now} at commands - something went wrong when executing a command. Text: {context.Message.Content} | Error: {result.ErrorReason}");
            }
        }
    }
}

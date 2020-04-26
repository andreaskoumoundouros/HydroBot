using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Hydrobot.Core.Data;
using Hydrobot.Resources.Database;
using System.Linq;
using System.Diagnostics;

/*
 TODO:
    - Differentiate between user-chosen names for the servers and the process names
     */

namespace Hydrobot.Core.Commands {
    public class ServerCommands : ModuleBase<SocketCommandContext> {
        [Group("server"), Alias("servers", "sv"), Summary("Group to manage various servers on bot host machine")]

        public class ServerGroup : ModuleBase<SocketCommandContext> {
            [Command("status"), Alias(""), Summary("Shows current running server status")]
            public async Task Me() {

                Process[] processes = Process.GetProcesses();

                List<string> serverProcessNames = Data.Data.GetServerProcessNames();

                var builder = new EmbedBuilder() {
                    Color = new Discord.Color(82, 186, 255),
                    Description = "**Current Server Statuses**"
                };

                foreach (var serverProcessName in serverProcessNames) {
                    var isUp = processes.Any(p => p.MainWindowTitle.ToLower().Contains(serverProcessName));

                    builder.AddField(x => {
                        x.Name = $"Process Name: {serverProcessName}";
                        x.Value = $"Description: {Data.Data.GetServerDescription(serverProcessName)}\n" + (isUp ? $" :white_check_mark: Online" : $" :x: Offline");
                        x.IsInline = false;
                    });
                }

                await Context.Channel.SendMessageAsync("", false, builder.Build());
            }

            [Command("start"), Summary("Start the server")]
            public async Task ServerStart([Remainder]string serverName = "minecraft server") {
                // Check to make sure that the desired server actually exists.
                List<string> serverNames = Data.Data.GetServerProcessNames();

                if (!Data.Data.GetServerProcessNames().Contains(serverName)) {
                    await Context.Channel.SendMessageAsync($"{serverName} is not an existing server!\n Existing servers are: {string.Join("\n", serverNames.ToArray())}");
                    return;
                }

                // Check to make sure the server isn't already up.
                foreach (var process in Process.GetProcesses()) {
                    if (process.MainWindowTitle.ToLower().Contains(serverName)) {
                        await Context.Channel.SendMessageAsync($"{serverName} is already up!");
                        return;
                    }
                }

                // Start the server and launch it in its own directory, set from the database.
                var startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = Data.Data.GetServerExecutableLocation(serverName);
                startInfo.FileName = Data.Data.GetServerExecutableName(serverName);
                startInfo.UseShellExecute = true;

                Process.Start(startInfo);
            }

            [Command("ip"), Alias("Address"), Summary("DMs the desired server IP address")]
            public async Task SendIp([Remainder]string serverName = "minecraft server") {
                // Check to make sure that the desired server actually exists.
                List<string> serverNames = Data.Data.GetServerProcessNames();

                if (!Data.Data.GetServerProcessNames().Contains(serverName)) {
                    await Context.Channel.SendMessageAsync($"{serverName} is not an existing server!\nExisting servers are:\n\t{string.Join("\n\t", serverNames.ToArray())}");
                    return;
                }

                try {
                    string externalIp = new System.Net.WebClient().DownloadString("http://icanhazip.com");
                    await Context.User.SendMessageAsync($"The server IP address for {serverName} is {externalIp.TrimEnd()}:{Data.Data.GetServerPort(serverName)}");
                } catch (Exception e) {
                    Console.WriteLine(e);
                    throw;
                }               
            }
        }
    }
}

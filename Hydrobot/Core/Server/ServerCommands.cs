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

namespace Hydrobot.Core.Commands {
    public class ServerCommands : ModuleBase<SocketCommandContext> {
        [Group("server"), Alias("servers", "sv"), Summary("Group to manage various servers on bot host machine")]

        public class ServerGroup : ModuleBase<SocketCommandContext> {
            [Command("status"), Alias(""), Summary("Shows current running server status")]
            public async Task Me(IUser User = null) {

                Process[] processes = Process.GetProcesses();

                // TODO: Store a registered list of server on the system and determine which of those are active.
                // Probably use the database dawg. Need to add servers table or some shit.

                List<string> serverNames = Data.Data.GetServerProcessNames();

                foreach (var process in processes) {
                    foreach (var serverName in serverNames) {
                        if (process.MainWindowTitle.ToLower().Contains(serverName)) {
                            await Context.Channel.SendMessageAsync($"{serverName} is up.");
                        }
                    }
                }

                foreach (var serverName in serverNames) {
                    var isUp = processes.Any(p => p.MainWindowTitle.ToLower().Contains(serverName));

                    if (isUp) {
                        await Context.Channel.SendMessageAsync($"{serverName} is up.");
                    } else {
                        await Context.Channel.SendMessageAsync($"{serverName} is not up.");
                    }
                }

            }

            [Command("start"), Summary("Start the server")]
            public async Task ServerStart(string serverName = null) {
                // TODO: actually use the serverName to have user specify which server to start.

                foreach (var process in Process.GetProcesses()) {
                    if (process.MainWindowTitle.ToLower().Contains("minecraft server")) {
                        await Context.Channel.SendMessageAsync($"The server is already up!");
                        return;
                    }
                }

                var startInfo = new ProcessStartInfo();
                startInfo.WorkingDirectory = Data.Data.GetServerExecutableLocation();
                startInfo.FileName = Data.Data.GetServerExecutableName();
                startInfo.UseShellExecute = true;

                Process.Start(startInfo);

            }

            [Command("reset"), Summary("Resets the server?")]
            public async Task Reset(IUser User = null) {
                await Context.Channel.SendMessageAsync($"No work yet :)");
                return;

                if (User == null) {
                    // No user has been mentioned
                    await Context.Channel.SendMessageAsync($":x: You need to mention which user to reset ounces for! e.g. !ounces reset {Context.User.Mention}");
                    return;
                }

                if (User.IsBot) {
                    await Context.Channel.SendMessageAsync(":x: Bots aren't people!");
                    return;
                }

                SocketGuildUser User1 = Context.User as SocketGuildUser;
                if (!User1.GuildPermissions.Administrator) {
                    await Context.Channel.SendMessageAsync(":x: You don't have the permissions to use this comand! Please ask a moderator to do so.");
                    return;
                }

                await Context.Channel.SendMessageAsync($":skull: {User.Mention}, you have been reset by {Context.User.Username}. Your ounces are now 0.");

                using (var DbContext = new SqliteDbContext()) {
                    DbContext.ounces.RemoveRange(DbContext.ounces.Where(x => x.UserId == User.Id));
                    await DbContext.SaveChangesAsync();
                }

            }

            [Command("ip"), Alias("Address"), Summary("DMs the server IP address")]
            public async Task SendIp() {
                try {
                    string externalIp = new System.Net.WebClient().DownloadString("http://icanhazip.com");
                    await Context.User.SendMessageAsync($"The server IP address is {externalIp}:25565");
                } catch (Exception e) {
                    Console.WriteLine(e);
                    throw;
                }               
            }
        }
    }
}

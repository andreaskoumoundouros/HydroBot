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

namespace Hydrobot.Core.Currency {
    public class ounces : ModuleBase<SocketCommandContext> {
        [Group("ounce"), Alias("ounces", "oz"), Summary("Group to manage user ounces")]

        public class OuncesGroup : ModuleBase<SocketCommandContext> {
            [Command(""), Alias("me", "my"), Summary("Shows your current ounces")]
            public async Task Me(IUser User = null) {
                if (User == null) {
                    await Context.Channel.SendMessageAsync($"{Context.User}, you currently have {Data.Data.GetOunces(Context.User.Id)} ounces! :sweat_drops:");
                } else {
                    await Context.Channel.SendMessageAsync($"{User.Username}, you currently have {Data.Data.GetOunces(User.Id)} ounces! :sweat_drops:");
                } 
            }

            [Command("give"), Alias("gift", "hydrate"), Summary("Used to give ounces")]
            public async Task Give(IUser User = null, int Amount = 0) {
                if (User == null) {
                    // No user has been mentioned
                    await Context.Channel.SendMessageAsync(":x: You didn't mention a user to give ounces to! e.g. !ounces **<@user>** <amount>");
                    return;
                }

                if (User.IsBot) {
                    await Context.Channel.SendMessageAsync(":x: Bots aren't people!");
                    return;
                }

                if (Amount == 0) {
                    await Context.Channel.SendMessageAsync(":x: You must specify an amount of ounces to give to " + User.Username);
                    return;
                }

                SocketGuildUser User1 = Context.User as SocketGuildUser;
                if (!User1.GuildPermissions.Administrator) {
                    await Context.Channel.SendMessageAsync(":x: You don't have the permissions to use this comand! Please ask a moderator to do so.");
                    return;
                }

                await Context.Channel.SendMessageAsync($":tada: {User.Mention} you have received **{Amount}** ounces from {Context.User.Username}!\n you currently have {Data.Data.GetOunces(Context.User.Id)} ounces!");

                await Data.Data.SaveOunces(User.Id, Amount);
            } 

            [Command("reset"), Summary("Resets the specified user's ounces")]
            public async Task Reset(IUser User = null) {
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
        }
    }
}

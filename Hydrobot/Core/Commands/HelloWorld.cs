using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Discord;
using Discord.Commands;
using Hydrobot.Resources.Extensions;

namespace Hydrobot.Core.Commands {
    public class HelloWorld : ModuleBase<SocketCommandContext> {
        [Command("hello"), Alias("helloworld", "ping"), Summary("basic helloworld/ping command")]

        public async Task HelloCommand() {
            await Context.Channel.SendMessageAsync("Hello World/ping!");
        }

        [Command("cat"), Summary("Sends a random cat picture?")]

        public async Task Cat() {
            WebRequest request = WebRequest.Create("https://cataas.com/cat");
            //WebResponse response = request.GetResponse();

            using (var response = await request.GetResponseAsync()) {
                using (var reader = new BinaryReader(response.GetResponseStream())) {

                    // Read file 
                    Byte[] bytes = await reader.ReadAllBytes();

                    // Write to local folder 
                    using (var fs = new FileStream("cat.jpeg", FileMode.Create)) {
                        await fs.WriteAsync(bytes, 0, bytes.Length);
                    }
                }
            }
            await Context.Channel.SendFileAsync("cat.jpeg");

        }

        [Command("embed"), Summary("embed command test")]
        public async Task Embed([Remainder]string Input = "None") {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
            Embed.WithColor(40, 170, 230);
            Embed.WithFooter("embed footer", Context.Guild.Owner.GetAvatarUrl());
            Embed.WithDescription("dis a link" + " [google](https://www.google.com)");

            Embed.AddField("User Input", Input);

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

    }
}

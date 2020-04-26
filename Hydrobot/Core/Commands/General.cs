using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Web.Script.Serialization;
using Discord;
using Discord.Commands;
using Hydrobot.Resources.Extensions;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Hydrobot.Core.Commands {
    public class General : ModuleBase<SocketCommandContext> {

        private readonly CommandService Service;

        public General(CommandService service) {
            Service = service;
        }

        [Command("help"), Summary("That's me! :heart_eyes:")]
        public async Task HelpAsync() {

            // TODO: Only show the commands that the user who sent the message can use.

            var builder = new EmbedBuilder() {
                Color = new Discord.Color(82, 186, 255),
                Description = "These are the commands you can use"
            };

            foreach (var module in Service.Modules) {
                string description = null;
                foreach (var cmd in module.Commands) {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"**{cmd.Aliases.First()}**\n{cmd.Summary}\n";
                }

                if (!string.IsNullOrWhiteSpace(description)) {
                    builder.AddField(x => {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            //await Context.Channel.SendMessageAsync("", false, builder.Build());

            await Context.Message.DeleteAsync();

            await Context.User.SendMessageAsync("", false, builder.Build());            
        }

        [Command("hello"), Alias("helloworld", "ping"), Summary("basic helloworld/ping command")]

        public async Task HelloCommand() {
            await Context.Channel.SendMessageAsync("Hello World/ping!");
        }

        [Command("cat"), Summary("Sends a random cat picture?")]

        public async Task Cat() {
            //Environment.GetEnvironmentVariable("TheCatApiKey", EnvironmentVariableTarget.User) // Get TheCatApi key from path if required.

            string imageUri;

            // Alternatively, use cataas which just returns a random cat image rather than needing to parse the json
            WebRequest request = WebRequest.Create("https://api.thecatapi.com/v1/images/search"); 

            using (var response = await request.GetResponseAsync()) {

                using (var reader = new BinaryReader(response.GetResponseStream())) {

                    // Read file 
                    Byte[] bytes = await reader.ReadAllBytes();

                    string rawJson = Encoding.Default.GetString(bytes);

                    JArray json = JArray.Parse(rawJson);
                    imageUri = Hydrobot.Resources.Extensions.Extensions.ExtractKeyValue(json, "url");

                    using (var webclient = new WebClient()) {
                        webclient.DownloadFile(imageUri, "cat.jpeg");
                    }

                    // Write to local folder
                    //        using (var fs = new FileStream("cat.jpeg", FileMode.Create)) {
                    //            await fs.WriteAsync(bytes, 0, bytes.Length);
                    //        }

                }
            }

            // TODO: Change the file structure to save bot-side assets in better locations than root. tehe. :p
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

        [Command("chris"), Summary("He's a big stinky head")]
        public async Task Chris() {

            await Context.Channel.SendMessageAsync("Chris is the biggest stinky head in history!");
        }

    }
}

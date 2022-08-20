using BTTT;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace BTTT.Services
{
    public class CommandHandler
    {

        public static IServiceProvider _provider;
        public static DiscordSocketClient _discord;
        public static CommandService _commands;
        public static IConfigurationRoot _config;
        public static bool loadedBot = false;

        public CommandHandler(DiscordSocketClient discord, CommandService commands, IConfigurationRoot config, IServiceProvider provider)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;

            _discord.Ready += OnReady;
            _discord.MessageReceived += OnMessageReceived;
            _discord.SlashCommandExecuted += SlashCommandHandler;
            _discord.ButtonExecuted += ButtonHandler;
            _discord.SelectMenuExecuted += SelectMenuExecuted;
            LoggingService log = new LoggingService(_discord, _commands);
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            
        }
        public async Task ButtonHandler(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "rules_agreement":
                    await component.DeferAsync();
                    var user = (IGuildUser)component.User;
                    if (user.RoleIds.ToList().Contains(1007731640170201209)) //Tester Role ID
                        return;

                    var role = StartupService._discord.GetGuild(984849800488964188).Roles.First(x => x.Name == "Tester");
                    await user.AddRoleAsync(role);
                    break;
            }
        }
        public async Task SelectMenuExecuted(SocketMessageComponent component)
        {
            switch (component.Data.CustomId)
            {
                case "grab_roles":
                    await component.DeferAsync();
                    var user = (IGuildUser)component.User;
                    var announcementsRole = StartupService._discord.GetGuild(984849800488964188).Roles.First(x => x.Name == "Announcements");
                    var ReleasesRole = StartupService._discord.GetGuild(984849800488964188).Roles.First(x => x.Name == "Releases");
                    var PollsRole = StartupService._discord.GetGuild(984849800488964188).Roles.First(x => x.Name == "Polls");

                    if (component.Data.Values.ToList().Contains("ping_announcements"))
                    {
                        if (!user.RoleIds.ToList().Contains(1010551407478313061)) //Announcements Role ID
                            await user.AddRoleAsync(announcementsRole);
                    }
                    else
                    {
                        if (user.RoleIds.ToList().Contains(1010551407478313061)) //Announcements Role ID
                            await user.RemoveRoleAsync(announcementsRole);
                    }

                    if (component.Data.Values.ToList().Contains("ping_releases"))
                    {
                        if (!user.RoleIds.ToList().Contains(1010551390642380870)) //Releases Role ID
                            await user.AddRoleAsync(ReleasesRole);
                    }
                    else
                    {
                        if (user.RoleIds.ToList().Contains(1010551390642380870)) //Releases Role ID
                            await user.RemoveRoleAsync(ReleasesRole);
                    }

                    if (component.Data.Values.ToList().Contains("ping_polls"))
                    {
                        if (!user.RoleIds.ToList().Contains(1010551427359314052)) //Polls Role ID
                            await user.AddRoleAsync(PollsRole);
                    }
                    else
                    {
                        if (user.RoleIds.ToList().Contains(1010551427359314052)) //Polls Role ID
                            await user.RemoveRoleAsync(PollsRole);
                    }

                    break;

                    
            }
        }


        private async Task OnMessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;

            if (message.Source != MessageSource.User) return;
            var msg = socketMessage as IUserMessage;

            if (msg.Author.Id == 272753984840663040)
            {
                if (msg.Content.StartsWith("help") || msg.Content.StartsWith("commands"))
                    await AdminCommands.Help(msg);
                else if (msg.Content.StartsWith("message") || msg.Content.StartsWith("msg"))
                    await AdminCommands.Message(msg);
                else if (msg.Content.StartsWith("temp"))
                {
                    var menuBuilder = new SelectMenuBuilder()
                       .WithPlaceholder("You have no roles selected.")
                       .WithCustomId("grab_roles")
                       .WithMinValues(0)
                       .WithMaxValues(3);
                    menuBuilder.AddOption("Announcements", "ping_announcements", "Click if you'd like to get pinged for announcements.", new Emoji("📯"));
                    menuBuilder.AddOption("Releases", "ping_releases", "Click if you'd like to get pinged for new releases.", new Emoji("🗞"));
                    menuBuilder.AddOption("Polls", "ping_polls", "Click if you'd like to get pinged for polls.", new Emoji("🗳"));

                    var builder = new ComponentBuilder()
                        .WithSelectMenu(menuBuilder);


                    await StartupService._discord.GetGuild(984849800488964188).GetTextChannel(1001926606593478706).
                        SendMessageAsync("Grab roles here:", components: builder.Build());
                }


            }

        }
        
        private async Task<Task> OnReady()
        {



            //var menuBuilder = new SelectMenuBuilder()
            //   .WithPlaceholder("Click to grab roles!")
            //   .WithCustomId("grab_roles")
            //   .WithMinValues(0)
            //   .WithMaxValues(3);
            //menuBuilder.AddOption("Announcements", "ping_announcements", "Click if you'd like to get pinged for announcements.", new Emoji("📯"));
            //menuBuilder.AddOption("New releases", "ping_releases", "Click if you'd like to get pinged for new releases.", new Emoji("🗞"));
            //menuBuilder.AddOption("Polls", "ping_polls", "Click if you'd like to get pinged for polls.", new Emoji("🗳"));

            //var builder = new ComponentBuilder()
            //    .WithSelectMenu(menuBuilder);


            //await StartupService._discord.GetGuild(984849800488964188).GetTextChannel(1001926606593478706).
            //    SendMessageAsync("Grab roles here:", components: builder.Build());

            return Task.CompletedTask;
        }
    }

}

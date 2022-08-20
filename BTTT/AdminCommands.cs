using BTTT;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace BTTT
{
    class AdminCommands
    {
        public static async Task Help(IUserMessage msg)
        {

            var embed = new EmbedBuilder()
             .WithTitle("Commands")
             .WithColor(new Color(52, 108, 237))
             .AddField("commands", "help/commands")
             .AddField("message ", "message/msg [channel ID] [content]")
             .Build();

            await msg.ReplyAsync(embed: embed);
        }
        public static async Task Message(IUserMessage msg)
        {
            var expectedArgs = new Type[] { typeof(ulong), typeof(string) };
            bool restOfMessageIsLastArg = true;
            object[] args = GetArgumentsFromMessage(msg, expectedArgs, restOfMessageIsLastArg).Result;
            if (args == null)
                return;

            await Services.StartupService._discord.GetGuild(984849800488964188).GetTextChannel((ulong)args[0]).SendMessageAsync((string)args[1]);
        }



        private async static Task<object[]> GetArgumentsFromMessage(IUserMessage msg, Type[] expectedArgs, bool restOfMessageIsLastArg = false)
        {
            string content = msg.Content;
            int spaceIndex = content.IndexOf(' ');
            content = content.Substring(spaceIndex + 1);
            bool lastArg = false;
            var returned_args = new object[expectedArgs.Length];
            int index = 0;
            foreach (Type expectedArg in expectedArgs) 
            {
                spaceIndex = content.IndexOf(' ');
                string arg = content;
                if (lastArg)
                {
                    await msg.ReplyAsync("Not enough arguments.");
                    return null;
                }
                if (!arg.Contains(' '))
                    lastArg = true;
                if (spaceIndex != -1)
                    arg = content.Substring(0, spaceIndex);
                try
                {
                    returned_args[index] = Convert.ChangeType(arg, expectedArg);
                }
                catch
                {
                    await msg.ReplyAsync("Invalid arguments. Use 'commands' for syntax.");
                    return null;
                }
                content = content.Substring(spaceIndex + 1);
                index++;
            }
            if (!lastArg)
            {
                if (restOfMessageIsLastArg)
                {
                    returned_args[index - 1] += " " + content;
                }
                else
                {
                    await msg.ReplyAsync("Too many arguments.");
                    return null;
                }
            }
            return returned_args;
        }




    }
}

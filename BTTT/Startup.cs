using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using BTTT.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BTTT
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        
        public Startup(string[] args)
        {
            var Builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddYamlFile("_config.yml");
            Configuration = Builder.Build();
        }
        public static async Task RunAsync(string[] args)
        {
            var Startup = new Startup(args);
            await Startup.RunAsync();
        }
        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<CommandHandler>();

            await provider.GetRequiredService<StartupService>().StartAsync();
            await Task.Delay(-1);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Warning,
                MessageCacheSize = 1000,
                GatewayIntents = Discord.GatewayIntents.All
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = Discord.LogSeverity.Warning,
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            }))
            .AddSingleton<CommandHandler>()
            .AddSingleton<StartupService>()
            .AddSingleton<LoggingService>()
            .AddSingleton(Configuration);
        }

    }
}

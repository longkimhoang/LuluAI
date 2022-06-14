using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LuluAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        configuration.Sources.Clear();

        IHostEnvironment environment = context.HostingEnvironment;

        configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables(prefix: "LuluAi_");
    })
    .ConfigureServices((services) =>
    {
        services
            .AddTransient<IDiscordClientLogger, DiscordClientLogger>();
    })
    .Build();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

// Get bot token and create the Discord client

string discordBotToken = config.GetSection("Discord").GetValue<string>("BotToken");
IDiscordClientLogger discordClientLogger = host.Services.GetRequiredService<IDiscordClientLogger>();

GatewayIntents gatewayIntents = GatewayIntents.AllUnprivileged;
gatewayIntents &= ~(GatewayIntents.GuildScheduledEvents);
gatewayIntents &= ~(GatewayIntents.GuildInvites);

DiscordSocketConfig discordSocketConfig = new()
{
    GatewayIntents = gatewayIntents,
};
using DiscordSocketClient client = new(discordSocketConfig);
client.Log += discordClientLogger.Log;

// Login and start listening to events

await client.LoginAsync(TokenType.Bot, discordBotToken);
await client.StartAsync();

using CommandService commandService = new();

CommandHandler commandHandler = new(client, commandService);
await commandHandler.InstallCommandsAsync();

await host.RunAsync();

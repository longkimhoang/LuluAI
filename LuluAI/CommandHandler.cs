using Amazon.S3;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LuluAI;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private IServiceProvider? _services;

    public CommandHandler(DiscordSocketClient client, CommandService commandService)
    {
        _client = client;
        _commandService = commandService;
    }

    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;

        _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commandService)
            .AddSingleton<IAmazonS3>(services =>
            {
                return new AmazonS3Client(Amazon.RegionEndpoint.APSoutheast1);
            })
            .AddLogging()
            .BuildServiceProvider();

        await _commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _services);
    }

    private async Task HandleCommandAsync(SocketMessage message)
    {
        if (message is SocketUserMessage userMessage)
        {
            if (userMessage.Author.IsBot)
            {
                return;
            }

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            if (userMessage.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                SocketCommandContext context = new(_client, userMessage);

                await _commandService.ExecuteAsync(context, argPos, _services);
            }
        }
    }
}

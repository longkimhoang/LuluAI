using Discord;
using Discord.Commands;

namespace LuluAI.CommandModules;

public class HelpModule : ModuleBase<SocketCommandContext>
{
    private readonly CommandService _commandService;

    public HelpModule(CommandService commandService)
    {
        _commandService = commandService;
    }

    [Command("help")]
    [Summary("Display this help message")]
    public async Task DisplayHelpAsync()
    {
        IEnumerable<CommandInfo> commands = _commandService.Commands;
        EmbedBuilder embedBuilder = new();

        foreach (CommandInfo command in commands)
        {
            embedBuilder.AddField(command.Name, command.Summary ?? "No description available");
        }

        await ReplyAsync("List of commands:", embed: embedBuilder.Build());
    }
}

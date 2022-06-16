using Discord.Commands;

namespace LuluAI.CommandModules;

public class HelloWorldModule : ModuleBase<SocketCommandContext>
{
    [Command("hello")]
    [Summary("A warm hello from your local donkey")]
    public async Task HelloWorldAsync()
    {
        await ReplyAsync("Hello from the other side");
    }
}

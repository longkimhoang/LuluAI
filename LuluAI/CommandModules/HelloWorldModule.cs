using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

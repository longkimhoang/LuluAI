using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace LuluAI;

public interface IDiscordClientLogger
{
    Task Log(LogMessage message);
}

public class DiscordClientLogger : IDiscordClientLogger
{
    private readonly ILogger<DiscordSocketClient> _logger;

    public DiscordClientLogger(ILogger<DiscordSocketClient> logger)
    {
        _logger = logger;
    }

    public Task Log(LogMessage message)
    {
        LogLevel level = LogLevelForSeverity(message.Severity);
        _logger.Log(level, message: "{message}", message.ToString());

        return Task.CompletedTask;
    }

    private static LogLevel LogLevelForSeverity(LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Debug,
        };
    }
}

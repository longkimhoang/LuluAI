using Amazon.S3;
using Amazon.S3.Model;
using Discord;
using Discord.Commands;
using LuluAI.Extensions;
using LuluAI.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LuluAI.CommandModules;

public class LuluedModule : ModuleBase<SocketCommandContext>
{
    private readonly IAmazonS3 _s3client;
    private readonly ILogger<LuluedModule> _logger;
    private readonly IConfiguration _config;
    private readonly Random _random;

    public LuluedModule(IAmazonS3 s3client, ILogger<LuluedModule> logger, IConfiguration config)
    {
        _s3client = s3client;
        _logger = logger;
        _config = config;
        _random = new();
    }

    [Command("lulued")]
    [Summary("Get lulued")]
    public async Task GetLuluedMemeAsync()
    {
        try
        {
            string? imageUrl = await GetRandomImageUrlAsync();

            if (imageUrl is null)
            {
                await ReplyAsync("Hold on. I got nothing to send to you.");
                return;
            }

            const ulong luluUserId = 803858731719983105;

            Embed embed = new EmbedBuilder()
                .WithImageUrl(imageUrl)
                .Build();

            await ReplyAsync($"Time for you to shine, {MentionUtils.MentionUser(luluUserId)}", embed: embed);
        }
        catch (AmazonS3Exception ex)
        {
            string? reply = string.Join(Environment.NewLine, "Encountered some problems executing this command.", ex.Message);
            await ReplyAsync(reply);
        }
    }

    private async Task<string?> GetRandomImageUrlAsync()
    {
        AmazonS3Options options = _config.GetSection(nameof(AmazonS3Options)).Get<AmazonS3Options>();

        const string prefix = "lulued/";

        ListObjectsV2Request listObjectsRequest = new()
        {
            BucketName = options.BucketName,
            Delimiter = "/",
            Prefix = prefix,
        };

        ListObjectsV2Response listObjectsResponse = await _s3client.ListObjectsV2Async(listObjectsRequest);
        var objects = listObjectsResponse.S3Objects
            .Where(o => o.Size > 0)
            .ToList();

        if (objects.Count == 0)
        {
            _logger.LogInformation("No objects in with prefix {prefix} in bucket. Skipping", prefix);
            return null;
        }

        int index = _random.Next(0, objects.Count);

        S3Object image = objects
            .ElementAt(index);

        return image.GetPublicUrl(options);
    }
}

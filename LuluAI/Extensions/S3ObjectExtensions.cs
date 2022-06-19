using Amazon.S3.Model;
using LuluAI.Options;

namespace LuluAI.Extensions;

public static class S3ObjectExtensions
{
    public static string GetPublicUrl(this S3Object s3Object, AmazonS3Options options)
    {
        return $"https://{options.BucketName}.s3.amazonaws.com/{s3Object.Key}";
    }
}

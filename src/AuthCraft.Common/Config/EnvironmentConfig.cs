namespace AuthCraft.Common.Config;

public class EnvironmentConfig
{
    public string ApplicationRootUrl { get; set; }

    public string BuildVersion { get; set; }

    public string AwsEnvironment => System.Environment.GetEnvironmentVariable("SYSTEM_ENV");

    public string AwsRegion => System.Environment.GetEnvironmentVariable("AWS_REGION");
}

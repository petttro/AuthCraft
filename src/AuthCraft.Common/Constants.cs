using System;

namespace AuthCraft.Common;

public static class Constants
{
    public static class EnvironmentVariables
    {
        public const string ConfigCrestUrl = "CONFIGCREST_URL";
    }

    public static class HeaderKeys
    {
        public const string XForwardedFor = "X-Forwarded-For";
        public const string RemoteAddr = "REMOTE_ADDR";
        public const string AuthCraftCorrelationIdHeader = "x-authcraft-correlationid";
    }

    public static class ContentType
    {
        public const string ApplicationJson = "application/json";
    }

    public static class CacheKeys
    {
        public static readonly string GlClientTokenCacheKey = "gl:client:token";
    }

    public static class CachePolicies
    {
        public const string GlConfigurationCachePolicy = "gl_configuration_cache_policy";
        public const string GlClientTokenCachePolicy = "gl_client_token_cache_policy";
    }

    public static class HttpValues
    {
        public const int DefaultHttpRequestTimeoutMilliseconds = 10000;
    }

    public static class Splunk
    {
        /// <summary>
        /// Template for Splunk Url
        /// </summary>
        public const string SplunkUrlTemplate =
            "https://pettro.splunkcloud.com/en-US/app/search/search?earliest={0}&latest={1}&q=search%20index%3D{2}%20Id%3D{3}%20";
    }

    public static class Cache
    {
        public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(90);
        public static readonly TimeSpan GlClientTokenTtl = TimeSpan.FromHours(6);
        public static readonly TimeSpan FallbackLifetime = TimeSpan.FromDays(4);
    }
}

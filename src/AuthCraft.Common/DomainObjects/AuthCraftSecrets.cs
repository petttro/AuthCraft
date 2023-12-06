using Newtonsoft.Json;

namespace AuthCraft.Common.DomainObjects;

public class AuthCraftSecrets
{
    [JsonProperty(PropertyName = "auth:Audience")]
    public string Audience { get; set; }

    [JsonProperty(PropertyName = "auth:ConfigCrestApiKey")]
    public string ConfigCrestApiKey { get; set; }

    [JsonProperty(PropertyName = "auth:Issuer")]
    public string Issuer { get; set; }

    public string Private { get; set; }
}

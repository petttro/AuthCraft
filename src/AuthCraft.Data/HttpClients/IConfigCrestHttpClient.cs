using AuthCraft.Data.Dto.ConfigCrest;

namespace AuthCraft.Data.HttpClients;

public interface IConfigCrestHttpClient
{
    Task<ConfigurationResponse> GetConfigurationAsync(string configurationSetId);
}

namespace AuthCraft.Data.HttpClients;

public interface IConfigCrestAuthHttpClient
{
    Task<string> GetClientTokenAsync(Guid apiKey);
}

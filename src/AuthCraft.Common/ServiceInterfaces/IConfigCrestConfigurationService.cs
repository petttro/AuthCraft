using System.Threading.Tasks;

namespace AuthCraft.Common.ServiceInterfaces;

public interface IConfigCrestConfigurationService
{
    Task<T> GetConfigItemAsync<T>(string configurationKey);
}

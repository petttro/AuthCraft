using System.Threading.Tasks;
using AuthCraft.Common.DomainObjects;

namespace AuthCraft.Common.Infrastructure.Aws;

public interface ISecretsService
{
    Task<SigningInfo> GetSigningKeyAsync();

    Task<string> GetApiKeyAsync();
}

using System.Threading.Tasks;
using AuthCraft.Common.DomainObjects;

namespace AuthCraft.Common.ServiceInterfaces;

public interface ISecurityTokenService
{
    Task<CreateTokenOutput> CreateAsync(ClientList.Client client);
}

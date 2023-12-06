using System;
using System.Threading.Tasks;
using AuthCraft.Common.DomainObjects;

namespace AuthCraft.Common.ServiceInterfaces;

public interface IClientAuthenticationService
{
    Task<ClientAuthenticationOutput> AuthenticateAsync(Guid key);
}

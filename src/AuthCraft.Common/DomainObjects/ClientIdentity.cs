using System.Collections.Generic;
using System.Security.Claims;

namespace AuthCraft.Common.DomainObjects;

public class ClientIdentity : AuthCraftIdentity
{
    public const string Type = "Client";

    public ClientIdentity(ClaimsIdentity claimsIdentity)
        : base(claimsIdentity, Type)
    {
    }

    public ClientIdentity(IEnumerable<Claim> claims, string authenticationType)
        : base(claims, authenticationType, Type)
    {
    }

    public ClientIdentity(IEnumerable<Claim> claims)
        : base(claims, Type)
    {
    }

    public virtual string Application => GetClaim(AuthCraftClaimTypes.Client.ApplicationName);
}

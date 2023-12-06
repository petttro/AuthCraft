using System;

namespace AuthCraft.Common.DomainObjects;

public class ClientAuthenticationOutput
{
    public string Application { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string Token { get; set; }
}

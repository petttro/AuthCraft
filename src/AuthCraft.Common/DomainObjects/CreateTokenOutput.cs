using System;

namespace AuthCraft.Common.DomainObjects;

public class CreateTokenOutput
{
    public string Token { get; set; }

    public DateTime ExpiresAt { get; set; }
}

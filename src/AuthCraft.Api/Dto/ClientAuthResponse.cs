using System;

namespace AuthCraft.Api.Dto;

public class ClientAuthResponse
{
    public string Application { get; set; }

    public string Token { get; set; }

    public DateTime ExpiresAt { get; set; }
}

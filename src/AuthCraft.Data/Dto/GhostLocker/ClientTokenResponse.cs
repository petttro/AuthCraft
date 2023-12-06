namespace AuthCraft.Data.Dto.ConfigCrest;

public class ClientTokenResponse
{
    public string Token { get; set; }

    public DateTime ExpiresAt { get; set; }
}

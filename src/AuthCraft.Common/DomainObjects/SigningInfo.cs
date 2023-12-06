namespace AuthCraft.Common.DomainObjects;

public class SigningInfo
{
    public string Audience { get; set; }

    public string Issuer { get; set; }

    public string Private { get; set; }
}

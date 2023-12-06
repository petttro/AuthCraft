namespace AuthCraft.Data.Dto.ConfigCrest;

public class ConfigurationResponse
{
    public string ConfigurationSetId { get; set; }

    public string Application { get; set; }

    public int ConfigurationVersion { get; set; }

    public string ConfigurationValue { get; set; }

    public DateTime LastUpdateDateTime { get; set; }

    public string LastChangedBy { get; set; }

    public string Comments { get; set; }

    public bool PublishedFlag { get; set; }

    public string ContentType { get; set; }

    public int CacheDurationSeconds { get; set; }
}

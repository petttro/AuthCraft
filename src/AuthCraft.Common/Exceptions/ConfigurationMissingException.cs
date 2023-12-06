namespace AuthCraft.Common.Exceptions;

public class ConfigurationMissingException : InternalSystemException
{
    public ConfigurationMissingException(string message)
        : base(message)
    {
    }
}

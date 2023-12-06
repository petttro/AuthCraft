using System;

namespace AuthCraft.Common.Exceptions;

public class InternalSystemException : Exception
{
    public InternalSystemException(string message)
        : base(message)
    {
    }

    public string CustomOverrideMessage { get; set; }
}

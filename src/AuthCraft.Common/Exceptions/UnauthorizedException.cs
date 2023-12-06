using System;

namespace AuthCraft.Common.Exceptions;

public class UnauthorizedException : BaseInputException
{
    public static readonly CustomErrorCode DefaultCustomErrorCode = CustomErrorCode.UnauthorizedDefault;

    public UnauthorizedException(string message)
        : base(message, DefaultCustomErrorCode)
    {
    }
}

using System;

namespace AuthCraft.Common.Exceptions;

public class NotFoundException : BaseInputException
{
    public static readonly CustomErrorCode DefaultCustomErrorCode = CustomErrorCode.NotFoundDefault;

    public NotFoundException(string message)
        : base(message, DefaultCustomErrorCode)
    {
    }
}

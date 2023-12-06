using System;
using System.Collections.Generic;

namespace AuthCraft.Common.Exceptions;

public class BaseInputException : Exception
{
    public readonly CustomErrorCode CustomErrorCode = CustomErrorCode.BadUserInputDefault;
    public IDictionary<string, object> InputErrors;

    public BaseInputException(string message)
        : base(message)
    {
    }

    public BaseInputException(string message, CustomErrorCode customErrorCode)
        : base(message)
    {
        CustomErrorCode = customErrorCode;
    }
}

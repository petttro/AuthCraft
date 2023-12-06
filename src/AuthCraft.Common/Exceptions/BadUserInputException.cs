using System.Collections.Generic;
using AuthCraft.Common.Extensions;

namespace AuthCraft.Common.Exceptions;

public class BadUserInputException : BaseInputException
{
    public BadUserInputException(string message)
        : base(message)
    {
    }

    public BadUserInputException(IDictionary<string, object> inputErrors)
        : base(CustomErrorCode.BadUserInputDefault.GetEnumDescription())
    {
        InputErrors = inputErrors;
    }
}

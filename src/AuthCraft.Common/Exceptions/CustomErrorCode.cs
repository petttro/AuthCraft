using System.ComponentModel.DataAnnotations;

namespace AuthCraft.Common.Exceptions;

public enum CustomErrorCode
{
    [Display(Description = "Internal server error")]
    InternalServerErrorDefault = 1000,

    [Display(Description = "Bad user input")]
    BadUserInputDefault = 1010,

    [Display(Description = "Not found")]
    NotFoundDefault = 1020,

    [Display(Description = "Forbidden")]
    ForbiddenDefault = 1040,

    [Display(Description = "Item already exists")]
    ItemAlreadyExistsDefault = 1050,

    [Display(Description = "Unauthorized")]
    UnauthorizedDefault = 1060
}

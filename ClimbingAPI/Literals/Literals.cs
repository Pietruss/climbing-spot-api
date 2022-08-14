using System.ComponentModel;

namespace ClimbingAPI.Literals
{
    public enum Literals
    {
        [Description("Authorization failed.")]
        AuthorizationFailed,

        [Description("Authorization failed.")]
        Info,

        [Description("Something went wrong.")]
        InternalError,

        [Description("New passowrd is the same as old one.Please change it.")]
        PasswordsAreIdentical,

        [Description("Invalid username or password.")]
        InvalidUsernameOrPassowrd
    }
}

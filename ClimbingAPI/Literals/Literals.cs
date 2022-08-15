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
        InvalidUsernameOrPassowrd,

        [Description("Invalid password.")]
        InvalidPassowrd,

        [Description("DELETE_USER")]
        DeleteUserAction,

        [Description("CHANGE_PASSWORD")]
        ChangePasswordAction,

        [Description("User assigned to climbing spot. Please remove climbing spot first and try again.")]
        UserAssignedToClimbingSpot
    }
}

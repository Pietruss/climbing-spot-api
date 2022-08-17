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
        UserAssignedToClimbingSpot,

        [Description("DELETE_CLIMBING_SPOT")]
        DeleteClimbingSpotAction,

        [Description("UPDATE_CLIMBING_SPOT")]
        UpdateClimbingSpotAction,

        [Description("CREATE_CLIMBING_SPOT")]
        CreateClimbingSpotAction,

        [Description("UPDATE_BOULDER")]
        UpdateBoulderAction,

        [Description("DELETE_ALL_BOULDERS")]
        DeleteAllBouldersAction,
        
        [Description("DELETE_BOULDER")]
        DeleteBoulderAction,

        [Description("CREATE_BOULDER")]
        CreateBoulderAction,

        [Description("Image not found.")]
        ImageNotFound,

        [Description("UPLOAD_IMAGE")]
        UploadImage,

        [Description("DELETE_IMAGE")]
        DeleteImageAction,

        [Description("Image not found.")]
        DeleteImage,

        [Description("Boulder has assigned image. Please delete the image and upload it again.")]
        BoulderHasAssingedImage,

        [Description("GET_IMAGE")]
        GetImageAction
    }
}

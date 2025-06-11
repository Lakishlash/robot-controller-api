namespace robot_controller_api.Models
{
    /// <summary>
    /// Payload for updating user credentials (email and/or password).
    /// </summary>
    public class UserCredentialUpdateModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}

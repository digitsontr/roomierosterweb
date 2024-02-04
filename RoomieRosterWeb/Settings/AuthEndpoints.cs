namespace RoomieRosterWeb.Settings
{
    public class AuthEndpoints
    {
        public string Register { get; set; }
        public string Login { get; set; }
        public string ForgotPassword { get; set; }
        public string ResetPassword { get; set; }
        public string CreateTokenByRefreshToken { get; set; }
        public string RevokeRefreshToken { get; set; }
        public string ConfirmEmail { get; set; }
    }
}


namespace Web.Api.Logic.Models.Account
{
    public class LoginResult
    {
        public string SessionGuid { get; set; }
        public string Token { get; set; }
        public bool MustResetPassword { get; set; }
        public bool MustVerifyEmail { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }

        public static LoginResult Get(string sessionGuid, bool mustResetPassword, bool mustVerifyEmail, string token = null)
        {
            return new LoginResult
            {
                Successful = true,
                SessionGuid = sessionGuid,
                Token = token,
                MustResetPassword = mustResetPassword,
                MustVerifyEmail = mustVerifyEmail,
            };
        }

        public static LoginResult Error(string message)
        {
            return new LoginResult
            {
                Successful = false,
                Message = message,
            };
        }
    }
}
using RoomieRosterWeb.Models;

namespace RoomieRosterWeb.Services
{
	public interface IAuthService
	{
		Task<object> LoginAsync(LogInDto logInDto);
        Task<object> RegisterAsync(SignUpDto signUpDto);
        Task<object> ConfirmEmail(string userId, string confirmationToken);
        Task<object> ForgotPassword(string email);
        Task<object> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<object> GetUser(string id, string accessToken);
    }
}


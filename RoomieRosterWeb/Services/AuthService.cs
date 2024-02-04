using Microsoft.Extensions.Options;
using RoomieRosterWeb.Models;
using RoomieRosterWeb.Settings;

namespace RoomieRosterWeb.Services
{
	public class AuthService: BaseService, IAuthService
	{
        private readonly MatchApiSettings _apiSettings;
        public AuthService(IOptions<MatchApiSettings> apiSettings)
		{
            _apiSettings = apiSettings.Value;
		}

        public async Task<object> ConfirmEmail(string userId, string confirmationToken)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendPostRequestAsync<TokenDto>(uri + _apiSettings.Endpoints.Auth.ConfirmEmail, new EmailConfirmatinDto()
            {
                Token = confirmationToken,
                UserId = userId
            } );

            return response;
        }

        public async Task<object> ForgotPassword(string email)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendPostRequestAsync<TokenDto>(uri + _apiSettings.Endpoints.Auth.ForgotPassword +"?email="+email, new object() { });

            return response;
        }

        public async Task<object> GetUser(string id, string accessToken)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendGetRequestAsync<UserDto>(uri + _apiSettings.Endpoints.User.GetById  + id, new { token = accessToken });

            return response;
        }

        public async Task<object> LoginAsync(LogInDto logInDto)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendPostRequestAsync<TokenDto>(uri + _apiSettings.Endpoints.Auth.Login, logInDto);

            return response;
        }

        public async Task<object> RegisterAsync(SignUpDto signUpDto)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendPostRequestAsync<TokenDto>(uri + _apiSettings.Endpoints.Auth.Register, signUpDto);

            return response;
        }

        public async Task<object> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendPostRequestAsync<TokenDto>(uri + _apiSettings.Endpoints.Auth.ResetPassword, resetPasswordDto);

            return response;
        }
    }
}


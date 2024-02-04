using Microsoft.Extensions.Options;
using RoomieRosterWeb.Models;
using RoomieRosterWeb.Settings;

namespace RoomieRosterWeb.Services
{
	public class FavouritesService:BaseService,IFavouritesService
	{
        private readonly MatchApiSettings _apiSettings;
        public FavouritesService(IOptions<MatchApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public async Task<object> Follow(string userId, string accessToken)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendPostRequestAsyncWithOptions<List<UserDto>>(uri + _apiSettings.Endpoints.User.Follow,new object { }, new { token = accessToken });

            return response;
        }

        public async Task<object> GetFollows(string accessToken)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendGetRequestAsync<List<UserDto>>(uri + _apiSettings.Endpoints.User.GetFollows, new { token = accessToken });

            return response;
        }

        public async Task<object> UnFollow(string userId, string accessToken)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendPostRequestAsyncWithOptions<List<UserDto>>(uri + _apiSettings.Endpoints.User.UnFollow + "?id=" + userId,new object { }, new { token = accessToken });

            return response;
        }
    }
}


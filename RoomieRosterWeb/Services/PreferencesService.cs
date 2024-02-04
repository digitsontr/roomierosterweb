using System;
using Microsoft.Extensions.Options;
using RoomieRosterWeb.Models;
using RoomieRosterWeb.Settings;

namespace RoomieRosterWeb.Services
{
	public class PreferencesService: BaseService,IPreferencesService
	{
        private readonly MatchApiSettings _apiSettings;
        public PreferencesService(IOptions<MatchApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        async Task<object> IPreferencesService.UpdatePreferences(UserPreferenecesDto preferencesDto, string accessToken)
        {
            var uri = new Uri(_apiSettings.BaseUrl);

            var response = await SendPutRequestAsyncWithOptions<MatchesDto>(uri + _apiSettings.Endpoints.Preferences, preferencesDto, new { token = accessToken });

            return response;
        }
    }
}


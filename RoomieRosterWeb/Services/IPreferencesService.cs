using System;
using RoomieRosterWeb.Models;

namespace RoomieRosterWeb.Services
{
	public interface IPreferencesService
	{
		public Task<object> UpdatePreferences(UserPreferenecesDto preferencesDto, string accessToken);
	}
}


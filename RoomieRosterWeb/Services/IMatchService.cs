using System;
using RoomieRosterWeb.Models;

namespace RoomieRosterWeb.Services
{
	public interface IMatchService
	{
        Task<object> GetMatchesAsync(string accessToken);
    }
}


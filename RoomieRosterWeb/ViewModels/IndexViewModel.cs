using System;
using RoomieRosterWeb.Models;

namespace RoomieRosterWeb.ViewModels
{
	public class IndexViewModel
	{
		public List<MatchDto> Matches { get; set; }
		public UserPreferenecesDto UserPreferences { get; set; }
	}
}


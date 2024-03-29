﻿namespace RoomieRosterWeb.Models
{
	public class UserDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string ProfilePhoto { get; set; }
		public byte Gender { get; set; }
		public UserPreferenecesDto Preferences { get; set; }
	}
}


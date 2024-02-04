using System;
namespace RoomieRosterWeb.Settings
{
    public class EndpointsSettings
    {
        public AuthEndpoints Auth { get; set; }
        public string Match { get; set; }
        public string Preferences { get; set; }
        public UserEndpoints User { get; set; }
    }
}


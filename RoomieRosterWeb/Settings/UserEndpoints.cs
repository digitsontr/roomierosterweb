using System;
namespace RoomieRosterWeb.Settings
{
    public class UserEndpoints
    {
        public string GetById { get; set; }
        public string GetFollows { get; set; }
        public string Follow { get; set; }
        public string UnFollow { get; set; }
        public string DisableUser { get; set; }
        public string EnableUser { get; set; }
    }
}


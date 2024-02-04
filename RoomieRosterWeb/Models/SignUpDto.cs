namespace RoomieRosterWeb.Models
{
	public class SignUpDto
	{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public DateTime BirthDay { get; set; }
        public byte Gender { get; set; }
        public string ProfilePhoto { get; set; } = "defaultimage.png";
    }
}


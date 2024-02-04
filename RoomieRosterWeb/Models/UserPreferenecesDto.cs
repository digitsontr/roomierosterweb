namespace RoomieRosterWeb.Models
{
	public class UserPreferenecesDto
	{
        public byte SmokingAllowed { get; set; }
        public byte GuestsAllowed { get; set; }
        public byte PetsAllowed { get; set; }
        public byte GenderPref { get; set; }
        public byte ForeignersAllowed { get; set; }
        public byte AlcoholAllowed { get; set; }
        public byte Duration { get; set; }
        public int AcceptableRoommatesMin { get; set; }
        public int AcceptableRoommatesMax { get; set; }
        public float BudgetMin { get; set; }
        public float BudgetMax { get; set; }
        public bool HasHome { get; set; }
        public AddressDto Address { get; set; }
    }
}


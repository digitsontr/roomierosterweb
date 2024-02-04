namespace RoomieRosterWeb.Services
{
	public interface IFavouritesService
	{
        Task<object> GetFollows(string accessToken);
        Task<object> Follow(string userId, string accessToken);
        Task<object> UnFollow(string userId, string accessToken);
    }
}


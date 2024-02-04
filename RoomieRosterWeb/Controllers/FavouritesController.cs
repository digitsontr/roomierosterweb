using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomieRosterWeb.Models;
using RoomieRosterWeb.Services;
using RoomieRosterWeb.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoomieRosterWeb.Controllers
{
    [Authorize]
    public class FavouritesController : Controller
    {
        private readonly IFavouritesService _favouritesService;
        public FavouritesController(IFavouritesService favouritesService)
        {
            _favouritesService = favouritesService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> IndexAsync()
        {
            var result = await _favouritesService.GetFollows(HttpContext.Request.Cookies["AccessToken"]);
          
            CustomResponseDto<List<UserDto>> favourites;


            if (result is CustomResponseDto<List<UserDto>>)
            {
                favourites = (CustomResponseDto<List<UserDto>>)result;
            }
            else
            {
                ModelState.Clear();

                CustomResponseDto<ErrorModel> errorResponse = result as CustomResponseDto<ErrorModel>;

                foreach (var error in errorResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                favourites = new CustomResponseDto<List<UserDto>>();
            }

            return View(favourites.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UnFollow(string Id)
        {
            var result = await _favouritesService.UnFollow(Id,HttpContext.Request.Cookies["AccessToken"]);

            if (result is CustomResponseDto<List<ErrorModel>>)
            {
                ModelState.Clear();

                CustomResponseDto<ErrorModel> errorResponse = result as CustomResponseDto<ErrorModel>;

                foreach (var error in errorResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View();
            }

            return RedirectToAction("Index");
        }
    }
}


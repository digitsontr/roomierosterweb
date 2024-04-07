using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomieRosterWeb.Models;
using RoomieRosterWeb.Services;
using RoomieRosterWeb.ViewModels;

namespace RoomieRosterWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMatchService _matchService;
    private readonly IAuthService _authService;
    private readonly IPreferencesService _preferencesService;

    public HomeController(ILogger<HomeController> logger, IMatchService matchService, IAuthService authService, IPreferencesService preferencesService)
    {
        _logger = logger;
        _matchService = matchService;
        _authService = authService;
        _preferencesService = preferencesService;
    }

    [Authorize]
    public async Task<IActionResult> AppAsync()
    {
        var result = await _matchService.GetMatchesAsync(HttpContext.Request.Cookies["AccessToken"]);
        var userInfoResult = await _authService.GetUser(User.FindFirstValue(ClaimTypes.NameIdentifier), HttpContext.Request.Cookies["AccessToken"]);
        CustomResponseDto<MatchesDto> matches;
        CustomResponseDto<UserDto> user;

        if (result is CustomResponseDto<MatchesDto>)
        {
            matches = (CustomResponseDto<MatchesDto>)result;
        }
        else
        {
            ModelState.Clear();

            CustomResponseDto<ErrorModel> errorResponse = result as CustomResponseDto<ErrorModel>;

            foreach (var error in errorResponse.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            matches = new CustomResponseDto<MatchesDto>();
        }

        if (userInfoResult is CustomResponseDto<UserDto>)
        {
            user = (CustomResponseDto<UserDto>)userInfoResult;
        }
        else
        {
            ModelState.Clear();

            CustomResponseDto<ErrorModel> errorResponse = userInfoResult as CustomResponseDto<ErrorModel>;

            foreach (var error in errorResponse.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            user = new CustomResponseDto<UserDto>();
        }



        return View(new IndexViewModel()
        {
            Matches = matches.Data.Matches,
            UserPreferences = user.Data.Preferences
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdatePreferences(UserPreferenecesDto userPreferenecesDto)
    {
        var result = await _preferencesService.UpdatePreferences(userPreferenecesDto, HttpContext.Request.Cookies["AccessToken"]);

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

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

      public IActionResult Index()
    {
        return View();
    }


    [Authorize]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


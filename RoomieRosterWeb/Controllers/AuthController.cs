using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RoomieRosterWeb.Models;
using RoomieRosterWeb.Services;
using RoomieRosterWeb.Helpers;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RoomieRosterWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthController(IAuthService authService, JwtTokenHelper jwtTokenHelper)
        {
            _authService = authService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        // GET: /<controller>/
        public IActionResult Login()
        {
            return View();
        }

        // POST: /<controller>/
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LogInDto loginDto)
        {
            var response = await
                _authService.LoginAsync(loginDto);

            if (response is CustomResponseDto<TokenDto>)
            {
                await SignInAsync(response);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.Clear();

                CustomResponseDto<ErrorModel> errorResponse = response as CustomResponseDto<ErrorModel>;

                foreach (var error in errorResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View();
            }
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["LogoutSuccessMessage"] = "Çıkış işlemi başarıyla tamamlandı, uygulamayı tekrar kullanmak için giriş yapmanız gerekli.";

            return RedirectToAction("Login");
        }

        // GET: /<controller>/
        public IActionResult Register()
        {
            return View();
        }

        // GET: /<controller>/
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(SignUpDto signUpDto)
        {
            var file = Request.Form.Files[0];

            string base64String = ConvertFileToBase64(file);

            signUpDto.ProfilePhoto = base64String;

           var response =
                await _authService.RegisterAsync(signUpDto);

            if (response is CustomResponseDto<ErrorModel>)
            {
                ModelState.Clear();

                CustomResponseDto<ErrorModel> errorResponse = response as CustomResponseDto<ErrorModel>;

                foreach (var error in errorResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View();
            }


            TempData["RegisterSuccessMessage"] = "Kayıt işlemi başarı ile tamamlandı giriş yapmak için email adresinizi doğrulamanız gerekiyor.";

            return RedirectToAction("Login");
        }

        // GET: /<controller>/
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /<controller>/
        [HttpPost]
        public async Task<IActionResult>? ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var response = await _authService.ForgotPassword(forgotPasswordDto.Email);

            if (response is CustomResponseDto<TokenDto>)
            {
                ViewBag.IsSuccess = true;
            }
            else
            {
                ModelState.Clear();

                CustomResponseDto<ErrorModel> errorResponse = response as CustomResponseDto<ErrorModel>;

                foreach (var error in errorResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            return View();
        }

        // GET: /<controller>/
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var response = await _authService.ResetPassword(resetPasswordDto);

            if (response is CustomResponseDto<TokenDto>)
            {
                await SignInAsync(response);

                ViewBag.IsSuccess = true;
            }
            else
            {
                ModelState.Clear();

                CustomResponseDto<ErrorModel> errorResponse = response as CustomResponseDto<ErrorModel>;

                foreach (var error in errorResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }


            ViewBag.UserId = resetPasswordDto.UserId;
            ViewBag.Token = resetPasswordDto.Token;

            return View("ResetPassword");
        }
        // GET: /<controller>/
        public async Task<IActionResult> ConfirmEmail()
        {
            string userIdFromQueryString = Request.QueryString.Value.Split("user=").Last().Split('&')[0];
            string confirmationToken  = Request.QueryString.Value.Split("token=").Last().Split('&')[0];

            var response = await _authService.ConfirmEmail(userIdFromQueryString, confirmationToken);

            if (response is CustomResponseDto<TokenDto>)
            {
                await SignInAsync(response);

                return View();
            }
            else
            {
                ModelState.Clear();

                CustomResponseDto<ErrorModel> errorResponse = response as CustomResponseDto<ErrorModel>;

                foreach (var error in errorResponse.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View();
            }
        }

        // GET: /<controller>/
        public IActionResult BlockedUser()
        {
            return View();
        }

        private string ConvertFileToBase64(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(fileBytes);

                return base64String;
            }
        }

        private async Task<bool> SignInAsync(object response)
        {
            CustomResponseDto<TokenDto> token = response as CustomResponseDto<TokenDto>;

            var principal = _jwtTokenHelper.GetPrincipalFromToken(token.Data.AccessToken);

            if (principal != null)
            {
                var identity = principal.Identity as ClaimsIdentity;
                var username = identity.FindFirst(ClaimTypes.Name)?.Value;
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.NameIdentifier, userId)
                        };
                var claimsIdentity = new ClaimsIdentity(claims, "cookie");
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                    IsPersistent = true
                };

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                    HttpOnly = true
                };


                HttpContext.Response.Cookies.Append("UserProfileImage", token.Data.User.ProfilePhoto, cookieOptions);
                HttpContext.Response.Cookies.Append("UserFirstName", token.Data.User.FirstName, cookieOptions);
                HttpContext.Response.Cookies.Append("UserLastName", token.Data.User.LastName, cookieOptions);
                HttpContext.Response.Cookies.Append("UserName", token.Data.User.Username, cookieOptions);
                HttpContext.Response.Cookies.Append("AccessToken", token.Data.AccessToken, new CookieOptions {
                    Expires = DateTime.Now.AddDays(1),
                    HttpOnly = false
                });
                HttpContext.Response.Cookies.Append("RefreshToken", token.Data.RefreshToken!, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                    HttpOnly = false
                });

                HttpContext.Session.SetString("UserProfileImage", token.Data.User.ProfilePhoto);
                HttpContext.Session.SetString("UserFirstName", token.Data.User.FirstName);
                HttpContext.Session.SetString("UserLastName", token.Data.User.LastName);
                HttpContext.Session.SetString("UserName", token.Data.User.Username);
                HttpContext.Session.SetString("AccessToken", token.Data.AccessToken);
                HttpContext.Session.SetString("RefreshToken", token.Data.RefreshToken!);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return true;
            }

            return false;
        }
    }
}


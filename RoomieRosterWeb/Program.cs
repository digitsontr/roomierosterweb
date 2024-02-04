using Microsoft.AspNetCore.Authentication.Cookies;
using RoomieRosterWeb.Helpers;
using RoomieRosterWeb.Services;
using RoomieRosterWeb.Settings;

var builder = WebApplication.CreateBuilder(args);


// Create Configurations
IConfiguration configuration = builder.Configuration.GetSection("MatchApi");
MatchApiSettings matchApiSettings = new MatchApiSettings();
configuration.Bind(matchApiSettings);

IConfiguration tokenConfiguration = builder.Configuration.GetSection("TokenOption");
TokenOptions tokenOptions = new TokenOptions();
configuration.Bind(tokenOptions);


// Add services to the container.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<JwtTokenHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPreferencesService, PreferencesService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IFavouritesService, FavouritesService>();
builder.Services.Configure<MatchApiSettings>(configuration);
builder.Services.Configure<TokenOptions>(tokenConfiguration);
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
       .AddCookie(options =>
       {
           options.LoginPath = "/Auth/Login";
           options.AccessDeniedPath = "/Auth/Login";
       });
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


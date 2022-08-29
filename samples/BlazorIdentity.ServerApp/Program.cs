using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorIdentity.ServerApp.Data;
using BlazorIdentity.ServerApp.Identity;
using BlazorStrap;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var expiryTimeSpan = builder.Configuration.GetValue<int>("CookieExpiryTimeSpanInMinutes");

builder.Services.AddBlazorServerIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false, expiryTimeSpan)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Force security ticket in the AuthN cookie to be revalidated every minute
builder.Services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.FromMinutes(1));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<AppUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddBlazorStrap();

// TODO: This is where we would add external login providers if following the normal ASPNetCore Identity pattern
// Values would be stored in user secrets for dev and hopefully KeyVault for a production application
//builder.Services.AddAuthentication()
//    .AddMicrosoftAccount(options =>
//    {
//        options.ClientId = builder.Configuration.GetValue<string>("Microsoft:ClientId")!;
//        options.ClientSecret = builder.Configuration.GetValue<string>("Microsoft:ClientSecret")!;
//        options.AuthorizationEndpoint = builder.Configuration.GetValue<string>("Microsoft:AuthorizationEndpoint")!;
//        options.TokenEndpoint = builder.Configuration.GetValue<string>("Microsoft:TokenEndpoint")!;
//    })
//    .AddGoogle(options =>
//    {
//        options.ClientId = builder.Configuration.GetValue<string>("Google:ClientId")!;
//        options.ClientSecret = builder.Configuration.GetValue<string>("Google:ClientSecret")!;
//    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorServerIdentityApi<AppUser>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();

using Blazored.LocalStorage;
using Blazored.Toast;
using Sedulur.Tools;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sedulur.Data;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Sedulur.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;

var builder = WebApplication.CreateBuilder(args);
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// ******
// BLAZOR COOKIE Auth Code (begin)
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

// BLAZOR COOKIE Auth Code (end)
// ******
// ******
// BLAZOR COOKIE Auth Code (begin)
// From: https://github.com/aspnet/Blazor/issues/1554
// HttpContextAccessor
//Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddTransient<AzureBlobHelper>();
builder.Services.AddTransient<PostCommentService>();
builder.Services.AddTransient<PostService>();
builder.Services.AddTransient<PostLikeService>();
builder.Services.AddTransient<RepostService>();
builder.Services.AddTransient<FollowService>();
builder.Services.AddTransient<TrendingService>();
builder.Services.AddTransient<LogService>();
builder.Services.AddTransient<ContactService>();

builder.Services.AddTransient<UserProfileService>();
builder.Services.AddTransient<SedulurDB>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET, PATCH, DELETE, PUT, POST, OPTIONS"));
});
var configBuilder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", optional: false);
IConfiguration Configuration = configBuilder.Build();

AppConstants.ProxyIP = Configuration["ProxyIP"];

var proxies = AppConstants.ProxyIP.Split(';');
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        foreach (var proxy in proxies)
        {
            options.KnownProxies.Add(IPAddress.Parse(proxy));
        }
    });



AppConstants.UploadUrlPrefix = Configuration["UploadUrlPrefix"];
AppConstants.SQLConn = Configuration["ConnectionStrings:SqlConn"];
AppConstants.RedisCon = Configuration["RedisCon"];
AppConstants.BlobConn = Configuration["ConnectionStrings:BlobConn"];
AppConstants.GMapApiKey = Configuration["GmapKey"];
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();

MailService.MailUser = Configuration["MailSettings:MailUser"];
MailService.MailPassword = Configuration["MailSettings:MailPassword"];
MailService.MailServer = Configuration["MailSettings:MailServer"];
MailService.MailPort = int.Parse(Configuration["MailSettings:MailPort"]);
MailService.SetTemplate(Configuration["MailSettings:TemplatePath"]);
MailService.SendGridKey = Configuration["MailSettings:SendGridKey"];
MailService.UseSendGrid = true;


SmsService.UserKey = Configuration["SmsSettings:ZenzivaUserKey"];
SmsService.PassKey = Configuration["SmsSettings:ZenzivaPassKey"];
SmsService.TokenKey = Configuration["SmsSettings:TokenKey"];


AppConstants.DefaultPass = Configuration["App:DefaultPass"];

AppConstants.StorageEndpoint = Configuration["Storage:Endpoint"];
AppConstants.StorageAccess = Configuration["Storage:Access"];
AppConstants.StorageSecret = Configuration["Storage:Secret"];
AppConstants.StorageBucket = Configuration["Storage:Bucket"];
var setting = new StorageSetting() { };
setting.Bucket = AppConstants.StorageBucket;
setting.SecretKey = AppConstants.StorageSecret;
setting.AccessKey = AppConstants.StorageAccess;

builder.Services.AddSingleton(setting);
builder.Services.AddTransient<StorageObjectService>();

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.MaximumReceiveMessageSize = 128 * 1024; // 1MB
});


var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    //app.UseHttpsRedirection();
    StaticWebAssetsLoader.UseStaticWebAssets(
              app.Environment,
              app.Configuration);

}


app.UseStaticFiles();

app.UseRouting();

// ******
// BLAZOR COOKIE Auth Code (begin)
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
// BLAZOR COOKIE Auth Code (end)
// ******

app.UseCors(x => x
.AllowAnyMethod()
.AllowAnyHeader()
.SetIsOriginAllowed(origin => true) // allow any origin  
.AllowCredentials());               // allow credentials 


#region auth api

/*
app.MapGet("/login", async (string username,string password) =>
{
    //[FromBody]LoginModel person
    LoginCls person = new LoginCls() { Username = username, Password = password };
    var output = new OutputCls() { IsSucceed = false };
    SedulurDB db = new();
    bool isAuthenticate = false;
    var usr = db.UserProfiles.Where(x => x.Username == person.Username).FirstOrDefault();
    if (usr != null)
    {
        var enc = new Encryption();
        var pass = enc.Decrypt(usr.Password);
        isAuthenticate = pass == person.Password;
    }
    // In this example we just log the user in
    // (Always log the user in for this demo)
    if (isAuthenticate)
    {
        // *** !!! This is where you would validate the user !!! ***
        // In this example we just log the user in
        // (Always log the user in for this demo)
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, person.Username),
                new Claim(ClaimTypes.Role, "Administrator"),
            };
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            
        };
        
        try
        {
            var res = Results.SignIn(
            new ClaimsPrincipal(claimsIdentity),
            authProperties, CookieAuthenticationDefaults.AuthenticationScheme);
            output.IsSucceed = true;
            output.Message = "success";
            return Results.Ok(output);
        }
        catch (Exception ex)
        {
            string error = ex.Message;
            Console.WriteLine(error);
        }
    }
    //if (!isAuthenticate) returnUrl = "/index?result=false";
    //return LocalRedirect(returnUrl);
    //return Results.Unauthorized();
    output.Message = "not authorized";
    return Results.Ok(output);
});
*/
#endregion

// BLAZOR COOKIE Auth Code (begin)
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
// BLAZOR COOKIE Auth Code (end)

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var db = new SedulurDB();
db.Database.EnsureCreated();

/*
var client = new HttpClient();
var storagesvc = new StorageObjectService(setting);
//var datablob = db.Posts.Where(x => !string.IsNullOrEmpty(x.ImageUrls)).Select(x => x.ImageUrls);
var datablob = db.UserProfiles.Where(x => !string.IsNullOrEmpty(x.PicUrl)).Select(x => x.PicUrl);
foreach(var filestr in datablob)
{
    var url = "https://sedulur.web.id" + filestr.Trim();
    var filebyte = await client.GetByteArrayAsync(url);
    var keystr = filestr.Replace("/api/dms/getfile?filename=", "");
    //File.WriteAllBytes(keystr, filebyte);

    var mime = MimeTypeHelper.GetMimeType(Path.GetExtension(keystr));
    var success = await storagesvc.InsertData(keystr, mime, filebyte);
    Console.WriteLine($"upload {keystr}:"+success);
}
*/

app.Run();

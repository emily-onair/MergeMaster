using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

using MergeMaster.Api.Data;
using MergeMaster.Api.Models;
using MergeMaster.Api.Services.GitHub;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOptions<GitHubOptions>()
    .Bind(builder.Configuration.GetSection(GitHubOptions.Section))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<DbOptions>()
    .Bind(builder.Configuration.GetSection(DbOptions.Section))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient<GitHubApi>((sp, http) =>
{
    var gh = sp.GetRequiredService<IOptions<GitHubOptions>>().Value;
    http.BaseAddress = new Uri(gh.BaseUrl);
    http.DefaultRequestHeaders.UserAgent.ParseAdd("MergeMaster/1.0"); // GitHub requires user agent
    http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
    http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", gh.ApiVersion);
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gh.Token);
}).AddStandardResilienceHandler();

builder.Services.AddRazorPages();

builder.Services.AddProblemDetails();

// builder.Services.AddSingleton<ReportStore>(); // temp in memory db
builder.Services.AddScoped<ReportStore>();

builder.Services.AddDbContext<ReportDb>((sp, options) =>
{
    var settings = sp.GetRequiredService<IOptions<DbOptions>>().Value;
    var env = sp.GetRequiredService<IHostEnvironment>();
    var path = Path.IsPathRooted(settings.Path)
        ? settings.Path
        : Path.Combine(env.ContentRootPath, settings.Path);
    options.UseSqlite($"Data Source={path}");
});


var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportDb>();
    await db.Database.MigrateAsync();
}

app.UseExceptionHandler();
app.MapControllers();


// var seedFile = app.Configuration["SeedFile"]; // dummy data for dev
// if (app.Environment.IsDevelopment() && !string.IsNullOrEmpty(seedFile))
// {
//     var path = Path.Combine(app.Environment.ContentRootPath, seedFile);
//     await using var stream = File.OpenRead(path);
//     var reports = await JsonSerializer.DeserializeAsync<List<Report>>(stream, JsonSerializerOptions.Web);
//     app.Services.GetRequiredService<ReportStore>().Seed(reports ?? []);
// }

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

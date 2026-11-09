using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

using MergeMaster.Api.Services.GitHub;
using Microsoft.EntityFrameworkCore;
using MergeMaster.Api.DAL.Repositories;
using MergeMaster.Api.DAL;

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

builder.Services.AddScoped<ReportStore>();
builder.Services.AddScoped<RepoStore>();
builder.Services.AddScoped<BranchStore>();
builder.Services.AddScoped<AuthorStore>();
builder.Services.AddScoped<TagStore>();

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

app.UseExceptionHandler();

app.MapControllers();

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

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReportDb>();
    await db.Database.MigrateAsync();
}

app.Run();

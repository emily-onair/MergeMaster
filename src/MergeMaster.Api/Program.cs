using MergeMaster.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<ReportStore>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapPost("/api/add", (NewReport input, ReportStore store) =>
{
    //  compare branch + SHA
    //      if 404 from GH -> error 404 commit or branch does not exist
    //      if status = ahead/diverged -> error 422 commit does not belong to this branch
    //  compare main + SHA
    //      if status = behind/identical -> error 422 commit already merged

    //  check for branch in db
    //      if branch -> check for commit
    //          if commit -> error 422 duplicate report
    //          else -> update existing report with new SHA
    //      else -> create new report
    var report = store.Add(input);
    return Results.Created($"/api/reports/{report.Id}", report);
});
app.MapGet("/api/reports", (ReportStore store) => store.All());

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

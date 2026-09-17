using SmartResumeAnalysisSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ResumeBuilderService>();
builder.Services.AddHttpClient<IResumeAiService, GeminiResumeAiService>();

var app = builder.Build();

var apiKey = builder.Configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
if (!string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine($"[DIAG] Gemini API key is configured (length {apiKey.Length})");
}
else
{
    Console.WriteLine("[DIAG] Gemini API key is NOT configured!");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

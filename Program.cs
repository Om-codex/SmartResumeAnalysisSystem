using SmartResumeAnalysisSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ResumeBuilderService>();
builder.Services.AddHttpClient<IResumeAiService, GeminiResumeAiService>();

var app = builder.Build();

var apiKey = builder.Configuration["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");
var geminiModel = builder.Configuration["Gemini:Model"] ?? "gemini-2.0-flash";
if (!string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine($"[DIAG] Gemini API key configured (length {apiKey.Length}), model: {geminiModel}");
}
else
{
    Console.WriteLine($"[DIAG] Gemini API key NOT configured! Model: {geminiModel}");
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

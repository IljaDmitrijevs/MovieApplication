using Client.Components;
using Client.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

var apiSettings = builder.Configuration.GetRequiredSection("ApiSettings").Get<ApiSettings>();

if (string.IsNullOrWhiteSpace(apiSettings?.BaseUrl))
{
    throw new InvalidOperationException("API base URL is not configured. Check ApiSettings:BaseUrl in appsettings.json.");
}

builder.Services.AddHttpClient("MovieApplicationApi", client =>
{
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MovieApplicationApi"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

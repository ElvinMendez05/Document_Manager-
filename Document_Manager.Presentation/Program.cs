using Document_Manager.Presentation.Components;
using Document_Manager.Presentation.Services;
using Document_Manager.Presentation.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Add Auth Services 
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<AuthViewModel>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthMessageHandler>();
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();

//API service
builder.Services.AddHttpClient<DocumentApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5058/");
});

//Auth Service 
builder.Services.AddHttpClient<AuthApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5058/api/");
})
.AddHttpMessageHandler<AuthMessageHandler>(); //That is important 

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

using ExampleAspnet.Middlewares;
using ExampleAspnet.Services;
using ExampleAspnet.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.IsEssential = true;

    // This works on localhost since the backend and the frontend is on the same "origin"
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddSingleton<ICache, Cache>();
builder.Services.AddHttpClient<IApiClient, ApiClient>();
builder.Services.AddSingleton<IOidcService, OidcService>();
builder.Services.AddSingleton<IBisOidcService, BisOidcService>();
builder.Services.AddSingleton<ErrorHandlerMiddleware>();

builder.Services.AddCors(options => // adds CORS policy
{
    options.AddPolicy("MyCorsPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("appsettings.json");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthorization();
app.UseSession();
app.UseCors("MyCorsPolicy");
app.MapControllers();
app.Run();
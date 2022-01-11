using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Modularz.Data.EF;
using Modularz.Data.Repository;
using Modularz.Models;
using StackExchange.Redis;
using UnitSense.CacheManagement;
using UnitSense.Repositories.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie = new CookieBuilder()
        {
            Name = "_modularz",
            SameSite = SameSiteMode.Lax,
            SecurePolicy = CookieSecurePolicy.SameAsRequest,
            MaxAge = TimeSpan.FromMinutes(260),
            IsEssential = true,
        };

        options.LoginPath = "/login";
        options.LogoutPath = "/";
    });
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/login";
    options.AccessDeniedPath = "/no";
    options.SlidingExpiration = true;
});
builder.Services.AddSwaggerGen();
builder.Host.ConfigureServices((context, collection) =>
{
    var configuration = builder.Configuration;
    string connectionString = configuration.GetSection("Database").Get<string>();
    var redisString = configuration.GetSection("Environnement").Get<EnvSettings>();
    BlogDbContext migratable = new BlogDbContext(connectionString);
    migratable.Database.Migrate();
    AppState.IsInit = migratable.IsInitialized();
    collection.AddDbContext<BlogDbContext>(options => options.UseNpgsql(connectionString));

    ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisString.RedisConnection);
    var redisCacheManager = new RedisCacheManager(multiplexer, redisString.EnvName);
    var localCacheManager = new LocalCacheManager();
    var busHandler = new RedisBusHandler(redisCacheManager, localCacheManager);
    collection.AddSingleton(redisCacheManager);
    collection.AddSingleton(localCacheManager);
    collection.AddSingleton(busHandler);
    collection.AddSingleton(new RepositorySetup() { EnvironnementPrefix = redisString.EnvName });
    collection.AddScoped<UserRepository>();
    collection.AddScoped<PostRepository>();
});

var app = builder.Build();
app.Use(async (context, next) =>
{
    var url = context.Request.Path.Value;

    // Rewrite privacy URL to index
    bool notAsStartup = NotAStartupUrl(url);
    if (!AppState.IsInit && notAsStartup && !url.Contains(".js") && !url.Contains(".css"))
    {
        // rewrite to startup page
        context.Request.Path = "/startup";
    }

    // Call the next delegate/middleware in the pipeline
    await next();
});

bool NotAStartupUrl(string? url)
{
    var uris = new[] { "/startup", "/start-init" };
    return !uris.Contains(url);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
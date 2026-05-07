using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using weddingcraft_be.Data;
using weddingcraft_be.Middleware;
using weddingcraft_be.Models;
using weddingcraft_be.Services;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Repositories;
using weddingcraft_be.Common.Helpers;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using weddingcraft_be.Models.Configuration;

var builder = WebApplication.CreateBuilder(args);

// ─── Serilog ────────────────────────────────────────────────────────────────

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .CreateLogger();

builder.Host.UseSerilog();

// ─── Options Pattern ─────────────────────────────────────────────────────────

builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("Gemini"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// ─── Database ────────────────────────────────────────────────────────────────

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Repositories ────────────────────────────────────────────────────────────

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();

// ─── Redis ───────────────────────────────────────────────────────────────────

var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>() 
    ?? throw new InvalidOperationException("Redis settings are not configured.");

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisSettings.ToConnectionString();
    options.InstanceName = "WeddingCraft";
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    try
    {
        var mux = ConnectionMultiplexer.Connect(redisSettings.ToConnectionString());
        Log.Information("Redis connected: {IsConnected} to {Host}", mux.IsConnected, redisSettings.Host);
        return mux;
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to connect to Redis at {Host}", redisSettings.Host);
        // In production, you might want to return a mock or handle this differently
        // For now, we allow it to throw or return a disconnected mux depending on requirements
        throw; 
    }
});

builder.Services.AddScoped<IRedisService, RedisService>();

// ─── Identity / JWT ──────────────────────────────────────────────────────────

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddSingleton<IJwtService, JwtService>();

var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
    throw new InvalidOperationException("JwtSettings:Secret is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

// ─── Application Services ────────────────────────────────────────────────────

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IContactMessageService, ContactMessageService>();
builder.Services.AddScoped<IChatService, ChatService>();

// ─── SignalR ─────────────────────────────────────────────────────────────────

builder.Services.AddSignalR();

// ─── Controllers / API ───────────────────────────────────────────────────────

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeddingCraft API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter: Bearer {your JWT token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// ─── CORS ────────────────────────────────────────────────────────────────────

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("default", p =>
        p.AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
         .WithOrigins(builder.Configuration["FrontendUrl"] ?? "http://localhost:5173"));
});

// ─── Build ───────────────────────────────────────────────────────────────────

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// ─── Auto-migrate & Seed ─────────────────────────────────────────────────────

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    SeedData.Initialize(db, hasher, config);
}

// ─── Middleware Pipeline ──────────────────────────────────────────────────────

app.UseSerilogRequestLogging();
app.UseMiddleware<RequestLoggingEnricherMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("default");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<weddingcraft_be.Hubs.ChatHub>("/hubs/chat");
app.MapControllers();

app.Run();

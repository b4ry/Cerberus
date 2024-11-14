using Cerberus.Api.Middlewares;
using Cerberus.Api.Options;
using Cerberus.Api.Services;
using Cerberus.Api.Services.Interfaces;
using Cerberus.DatabaseContext;
using Cerberus.DatabaseContext.Repositories;
using Cerberus.DatabaseContext.Repositories.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Cerberus API",
        Description = "An ASP.NET Core Web API for managing logging funcionality.",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddScoped<JwtSecurityTokenGenerator>();
builder.Services.AddScoped<ISecurityTokenGeneratorFactory, SecurityTokenGeneratorFactory>();
builder.Services.AddScoped(x => {
    var configuration = x.GetRequiredService<IConfiguration>();
    var tokenType = configuration.GetValue<string>("TokenGenerator:Type");

    var factory = x.GetRequiredService<ISecurityTokenGeneratorFactory>();
    var tokenGenerator = factory.Create(tokenType!);

    return tokenGenerator;
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtConfigurationSectionService, JwtConfigurationSectionService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var rateLimitConfigurationSection = new RateLimitConfigurationSection();
builder.Configuration.GetSection("RateLimitOptions").Bind(rateLimitConfigurationSection);

builder.Services.AddRateLimiter(_ => _
    .AddSlidingWindowLimiter(policyName: rateLimitConfigurationSection.PolicyName, options =>
    {
        options.PermitLimit = rateLimitConfigurationSection.PermitLimit;
        options.Window = TimeSpan.FromSeconds(rateLimitConfigurationSection.Window);
        options.SegmentsPerWindow = rateLimitConfigurationSection.SegmentsPerWindow;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = rateLimitConfigurationSection.QueueLimit;
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
    context!.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseRateLimiter();
app.UseCors("AllowReactApp");
app.MapControllers().RequireRateLimiting(rateLimitConfigurationSection.PolicyName); ;

app.Run();

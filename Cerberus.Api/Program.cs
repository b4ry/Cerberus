using Cerberus.Api.Services;
using Cerberus.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.MapControllers();

app.Run();

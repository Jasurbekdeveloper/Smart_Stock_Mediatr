using SmartStock.Application;
using SmartStock.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SmartStock.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "SmartStock API", Version = "v1" });
});

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<SmartStock.WebAPI.Services.JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<SmartStock.WebAPI.Middleware.ExceptionHandlingMiddleware>();
builder.Services.AddScoped<SmartStock.Application.Common.Interfaces.ICurrentUserService, SmartStock.WebAPI.Services.CurrentUserService>();
builder.Services.AddScoped<SmartStock.WebAPI.Services.IJwtTokenService, SmartStock.WebAPI.Services.JwtTokenService>();
builder.Services.AddHostedService<SmartStock.WebAPI.Services.IdentitySeederHostedService>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<SmartStock.WebAPI.Services.JwtOptions>()
                  ?? throw new InvalidOperationException("Jwt section is missing.");

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwt.Key))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<SmartStock.WebAPI.Middleware.ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

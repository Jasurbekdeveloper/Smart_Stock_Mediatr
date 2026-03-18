using Microsoft.AspNetCore.Identity;
using SmartStock.Domain.Identity;

namespace SmartStock.WebAPI.Services;

public class IdentitySeederHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdentitySeederHostedService> _logger;
    private readonly IConfiguration _configuration;

    public IdentitySeederHostedService(IServiceProvider serviceProvider, ILogger<IdentitySeederHostedService> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { Roles.Admin, Roles.Sotuvchi })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        var adminUserName = _configuration["Seed:AdminUserName"] ?? "admin";
        var adminPassword = _configuration["Seed:AdminPassword"] ?? "Admin123!";

        var admin = await userManager.FindByNameAsync(adminUserName);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = adminUserName
            };

            var res = await userManager.CreateAsync(admin, adminPassword);
            if (!res.Succeeded)
            {
                _logger.LogError("Admin create failed: {Errors}", string.Join(", ", res.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(admin, Roles.Admin))
            await userManager.AddToRoleAsync(admin, Roles.Admin);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}


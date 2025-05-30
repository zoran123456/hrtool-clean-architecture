using HRTool.API.Extensions;
using HRTool.API.Services;
using HRTool.API.Utils;
using HRTool.API.Middleware;
using HRTool.Application;
using HRTool.Application.Services;
using HRTool.Domain.Interfaces;
using HRTool.Infrastructure.Data;
using HRTool.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace HRTool.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<HrDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddProjectRepositories();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<LinkService>();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerWithJwt();

            var app = builder.Build();

            // Seed admin user if none exists
            await SeedAdminUserIfMissing(app.Services);

            // Conditionally seed test data
            await SeedTestDataIfEnabled(app.Services);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use global exception handler
            app.UseCustomExceptionHandler(app.Services.GetRequiredService<ILoggerFactory>());

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static async Task SeedAdminUserIfMissing(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HrDbContext>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await AdminSeeder.SeedAdminIfNoneExistsAsync(db, userRepo, unitOfWork);
        }

        private static async Task SeedTestDataIfEnabled(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            if (config.GetValue<bool>("EnableTestDataSeeding"))
            {
                var db = scope.ServiceProvider.GetRequiredService<HrDbContext>();
                await DataSeeder.SeedTestDataAsync(db);
            }
        }
    }
}

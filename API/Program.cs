using Core.Repositories.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Data;
using Project.APIS.Extensions;
using Project.APIS.MiddleWare;
using Repository.Data.Identity;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Project.Repository.Data.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddApplicationServices();

            builder.Services.AddIdentity<AppUser, IdentityRole>(Options => { })
                    .AddEntityFrameworkStores<AppIdentityDbContext>();

            // ✅ Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssure"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Authkey"] ?? string.Empty))
                };
            });

            builder.WebHost.UseUrls("http://localhost:7069");
            var app = builder.Build();


            using var scop = app.Services.CreateScope();
            var Services = scop.ServiceProvider;
            var _dbcontext = Services.GetRequiredService<StoreContext>();
            var _IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
            var _userManger = Services.GetRequiredService<UserManager<AppUser>>();
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _dbcontext.Database.MigrateAsync();
                await _IdentityDbContext.Database.MigrateAsync();
                await AppIdentityDbContextSeed.SeedUserAsync(_userManger);
            }
            catch (Exception ex)
            {
                var logger = LoggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration");
            }

            app.UseMiddleware<ExcptionMiddleWare>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseHttpsRedirection();

            // ✅ Use CORS before authorization
            app.UseCors("AllowReactApp");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();
            app.MapControllers();
            app.Run();
        }
    }
}

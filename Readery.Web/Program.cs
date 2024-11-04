using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Readery.Core.Models.Identity;
using Readery.Core.Repositores;
using Readery.DataAccess.Data;
using Readery.DataAccess.Data.Seeder;
using Readery.DataAccess.Repositories;
using Readery.Utilities.Interfaces;
using Readery.Utilities.Services;
using Stripe;


namespace Readery.Web
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(
                    builder.Configuration.GetConnectionString("default")
                ));

            builder.Services.AddLogging();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddHttpClient();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            }
            );

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;

            }).AddFacebook(options =>
            {

                options.AppId = "854501606877667";
                options.AppSecret = "8e55edacd2c8db1b7b20ed5cc530b4bf";
                options.Fields.Add("name");

                options.Events = new OAuthEvents
                {
                    OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/Account/Login"); // Redirect to login page
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    }
                };

            }).AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Google:ClientSecret"];
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly)
             .AddFluentValidationAutoValidation();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(


            )
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole,
                   ApplicationDbContext, int>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, int>>()
            .AddDefaultTokenProviders(); // Add token providers for password reset, email confirmation, etc

            builder.Services.AddAutoMapper(typeof(Readery.Core.Repositores.IUnitOfWork).Assembly);

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser().Build();

            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Account/Login";
                options.AccessDeniedPath = "/Account/Account/AccessDenied";
            });

            builder.Services.AddSingleton<PayPalService>();
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                try
                {
                    context.Database.Migrate();
                    await SeedData.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it accordingly
                    Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
                }
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

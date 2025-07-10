
using System.Text;
using DataAccessObjects.DB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.Implementations;
using Services.Interfaces;
using WebAPI.Services;
using Microsoft.AspNetCore.Authentication.OAuth;
using RentNest.Infrastructure.DataAccess;
using BusinessObjects.Consts;
using BusinessObjects.Configs;
using DataAccessObjects;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // ======= DATABASE =======
            builder.Services.AddDbContext<RentNestSystemContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // ======= DEPENDENCY INJECTION =======
            // DAO
            builder.Services.AddScoped<AccommodationDAO>();
            builder.Services.AddScoped<AccountDAO>();
            builder.Services.AddScoped<UserProfileDAO>();
            builder.Services.AddScoped<PostDAO>();
            // Repository
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<Repositories.Interfaces.IAccommodationRepository, Repositories.Implements.AccommodationRepository>();
            builder.Services.AddScoped<Repositories.Interfaces.IPostRepository, Repositories.Implements.PostRepository>();
            // Service
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<Service.Interfaces.IAccommodationService, Service.Implements.AccommodationService>();
            builder.Services.AddScoped<Service.Interfaces.IPostService, Service.Implements.PostService>();
            // builder.Services.AddScoped<IAzureOpenAIService, AzureOpenAIService>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddHttpContextAccessor();
            // ======= AUTHENTICATION =======
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/auth/login"; // hoặc trang login của bạn
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "accessToken";
                options.Cookie.SameSite = SameSiteMode.Lax;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    ),
                    ValidateIssuerSigningKey = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        ctx.Request.Cookies.TryGetValue("accessToken", out var token);
                        if (!string.IsNullOrEmpty(token))
                        {
                            ctx.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            // ======= SESSION =======
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // ======= CORS =======
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5048", "https://localhost:7031")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthSchemes.Cookie;
                options.DefaultSignInScheme = AuthSchemes.Cookie;
            })
            .AddCookie(AuthSchemes.Cookie, config =>
            {
                config.LoginPath = "/Auth/Login";
                config.AccessDeniedPath = "/Auth/AccessDenied";
                config.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Thời gian hết hạn cookie
            })
            .AddGoogle(AuthSchemes.Google, options =>
            {
                options.ClientId = authSettings.Google.ClientId;
                options.ClientSecret = authSettings.Google.ClientSecret;
                options.CallbackPath = "/Auth/signIn-google";
            })
            .AddFacebook(AuthSchemes.Facebook, options =>
            {
                options.AppId = authSettings.Facebook.AppId;
                options.AppSecret = authSettings.Facebook.AppSecret;
                options.CallbackPath = "/Auth/signIn-facebook";
                options.Events = new OAuthEvents
                {
                    OnRemoteFailure = context =>
                    {
                        context.Response.Redirect("/Auth/Login");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

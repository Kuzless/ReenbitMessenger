using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyMessenger.Domain;
using MyMessenger.Domain.Entities;
using MyMessenger.Domain.Interfaces;
using MyMessenger.Domain.Repositories;
using MyMessenger.Application.Services;
using MyMessenger.Application.Services.Interfaces;
using MyMessenger.Application.Services.JwtAuth;
using MyMessenger.Application.Services.JwtAuth.Interfaces;
using MyMessenger.Application.ÑommandsQueries.Users.Queries;
using MyMessenger.Options;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyMessenger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // 

            builder.Services.AddDbContext<DatabaseContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseLazyLoadingProxies();
            });
            builder.Services.AddIdentity<User, IdentityRole>(options =>
                    {
                        options.User.RequireUniqueEmail = true;
                    })
                .AddTokenProvider<UserAuthProvider<User>>("MyMessenger")
                .AddEntityFrameworkStores<DatabaseContext>();

            //

            builder.Services.AddAutoMapper(typeof(AutoMappingProfile)); 
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISignUpService, SignUpService>();
            builder.Services.AddScoped<IJWTGeneratorService, JWTGeneratorService>();
            builder.Services.AddScoped<IJWTRetrievalService, JWTRetrievalService>();

            //

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginQueryHandler).Assembly));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                };
            });

            var app = builder.Build();
    
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

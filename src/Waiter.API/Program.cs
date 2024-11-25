using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Waiter.API.Exceptions;
using Waiter.Application.Models;
using Waiter.Domain.Models;
using Waiter.Infra.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();

var jwtSecretKey = builder.Configuration["JWT:Key"];

if (string.IsNullOrWhiteSpace(jwtSecretKey))
    throw new InvalidConfigurationException("JWT:Key");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AppDb")
);

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(c =>
    {
        c.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        };
    });

builder.Services.AddAuthorizationBuilder();

builder
    .Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseExceptionHandler(options => { });

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Map(
        "/",
        async context =>
        {
            context.Response.StatusCode = StatusCodes.Status308PermanentRedirect;
            context.Response.Redirect("/swagger");
            await context.Response.BodyWriter.CompleteAsync();
        }
    );
}
else
{
    app.Map(
        "/",
        async context =>
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsJsonAsync(new MessageResponse("Welcome to Waiter API!"));
        }
    );
}

app.Run();

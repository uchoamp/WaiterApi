using Waiter.API;
using Waiter.Application.Models.Common;
using Waiter.Infra.Data;

var builder = WebApplication.CreateBuilder(args);

ValidateSettings.Validate(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

await app.InitialiseDatabaseAsync();

app.UseAuthentication();
app.UseAuthorization();

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

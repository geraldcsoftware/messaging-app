using FastEndpoints;
using Humanizer;
using MessagingApp.Api.Authorization;
using MessagingApp.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration, "Logging");
    config.WriteTo.Console(theme: AnsiConsoleTheme.Code,
                           outputTemplate:
                           "{Timestamp:HH:mm:ss} [{Level} - {SourceContext}] {Message}{NewLine}{Exception}");
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(options =>
{
    options.RequestBodyLogLimit = (int)5.Kilobytes().Bytes;
    options.ResponseBodyLogLimit = (int)5.Kilobytes().Bytes;
    options.LoggingFields = HttpLoggingFields.RequestPath        |
                            HttpLoggingFields.RequestMethod      |
                            HttpLoggingFields.RequestHeaders     |
                            HttpLoggingFields.RequestBody        |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.ResponseBody;
    options.MediaTypeOptions.Clear();
    options.MediaTypeOptions.AddText("application/json");
    options.RequestHeaders.Add("Content-Type");
    options.RequestHeaders.Add("Accept");
    options.RequestHeaders.Add("Authorization");
});

builder.Services.AddFastEndpoints();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Authentication:Authority"];
            options.Audience = builder.Configuration["Authentication:Audience"];
        });

builder.Services.AddSingleton<IAuthorizationHandler, ClientIdAuthorizationRequirementHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientUser", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("ClientId");
        policy.AddRequirements(new ClientIdRequirement());
    });
});


builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var connectionString = builder.Configuration.GetConnectionString("DbConnection") ??
                           throw new Exception("Connection string not properly configured");
    options.UseSqlServer(connectionString);
    options.UseLoggerFactory(loggerFactory);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseFastEndpoints(options => { options.RoutingOptions = routes => routes.Prefix = "api"; });
app.Run();
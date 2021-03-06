using FastEndpoints;
using Humanizer;
using MessagingApp.Api.Authorization;
using MessagingApp.Api.Configuration;
using MessagingApp.Api.Mapping;
using MessagingApp.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Templates;

const string LogTemplate = "{@t:yyyy/MM/dd HH:mm:ss} [{@l} - {SourceContext}] {@m}{NewLine}{#if IsDefined(@x)}{@x}{NewLine}{#end}";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    var expressionTemplate = new ExpressionTemplate(LogTemplate);
    config.ReadFrom.Configuration(context.Configuration, "Logging");
    config.WriteTo.Console(expressionTemplate);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpLogging(options =>
                                    {
                                        options.RequestBodyLogLimit = (int)5.Kilobytes().Bytes;
                                        options.ResponseBodyLogLimit = (int)5.Kilobytes().Bytes;
                                        options.LoggingFields =
                                            HttpLoggingFields.RequestPath | HttpLoggingFields.RequestMethod
                                                                          | HttpLoggingFields.RequestHeaders
                                                                          | HttpLoggingFields.RequestBody
                                                                          | HttpLoggingFields.ResponseStatusCode
                                                                          | HttpLoggingFields.ResponseBody;
                                        options.MediaTypeOptions.Clear();
                                        options.MediaTypeOptions.AddText("application/json");
                                        options.RequestHeaders.Add("Content-Type");
                                        options.RequestHeaders.Add("Accept");
                                        options.RequestHeaders.Add("Authorization");
                                    });
}

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

builder.Services.Configure<PagingOptions>(builder.Configuration.GetSection("Paging"));
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var connectionString = builder.Configuration.GetConnectionString("DbConnection") ??
                           throw new Exception("Connection string not properly configured");
    switch (builder.Configuration["DbProvider"])
    {
        case "Postgres":
            options.UseNpgsql(connectionString);
            break;
        default:
            options.UseSqlServer(connectionString);
            break;
    }

    options.UseLoggerFactory(loggerFactory);
});
builder.Services.AddTransient<IIdGenerator, RandomIdGenerator>();
builder.Services.AddMapper();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpLogging();
}

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseFastEndpoints(options => options.RoutingOptions = routes => routes.Prefix = "api");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.Run();
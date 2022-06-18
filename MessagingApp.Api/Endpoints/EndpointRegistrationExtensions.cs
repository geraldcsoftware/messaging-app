namespace MessagingApp.Api.Endpoints;

public static class EndpointRegistrationExtensions
{
    public static void UseAppEndpoints(this WebApplication app)
    {
        // schedule campaign
        app.MapPost("api/v1/schedules", () => Results.Ok()).RequireAuthorization();
        
        // cancel schedule
        app.MapDelete("api/v1/schedules/{id}", () => Results.Ok()).RequireAuthorization();
        
        // get schedules
        app.MapGet("api/v1/schedules", () => new[] { "Gerald", "Chifanzwa" });
    }
}
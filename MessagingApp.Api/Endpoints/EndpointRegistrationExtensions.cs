namespace MessagingApp.Api.Endpoints;

public static class EndpointRegistrationExtensions
{
    public static void UseAppEndpoints(this WebApplication app)
    {

        
        
        // get campaigns
        app.MapGet("api/v1/clients/{clientId}/campaigns", () => new[] { "Gerald", "Chifanzwa" });
        
        // get campaign by id
        app.MapGet("api/v1/clients/{clientId}/campaigns/{id}", () => Results.Ok());
        
        // create campaign
        app.MapPost("api/v1/clients/{clientId}/campaigns", () => Results.Ok()).RequireAuthorization();
        
        // edit campaign
        app.MapPut("api/v1/clients/{clientId}/campaigns/{id}", () => Results.Ok()).RequireAuthorization();
        
        // delete campaign
        app.MapDelete("api/v1/clients/{clientId}/campaigns/{id}", () => Results.Ok()).RequireAuthorization();  
        
        
        // schedule campaign
        app.MapPost("api/v1/schedules", () => Results.Ok()).RequireAuthorization();
        
        // cancel schedule
        app.MapDelete("api/v1/schedules/{id}", () => Results.Ok()).RequireAuthorization();
        
        // get schedules
        app.MapGet("api/v1/schedules", () => new[] { "Gerald", "Chifanzwa" });
    }
}
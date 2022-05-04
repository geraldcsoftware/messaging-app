using Microsoft.AspNetCore.Authorization;

namespace MessagingApp.Api.Authorization;

public class ClientIdAuthorizationRequirementHandler: AuthorizationHandler<ClientIdRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
                                                   ClientIdRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext) return Task.CompletedTask;
        
        var clientId = context.User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value;

        httpContext.Request.Headers.TryGetValue("ClientId", out var clientIdHeader);
        if (clientId == clientIdHeader)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
using Microsoft.AspNetCore.Mvc;

namespace MessagingApp.Api.ViewModels;

public class UpdateClientRequest
{
    [FromRoute(Name =  "id")]
    public string? Id { get; set; }

    public string? Name { get; set; }
}
namespace MessagingApp.Infrastructure.Models;

public class MessageCampaign
{
    public string? Id { get; set; }
    public string? ClientId { get; set; }
    public DateTime Created { get; set; }
    public string? Name { get; set; }
    public string? Template { get; set; }
    public Client? Client { get; set; }
}
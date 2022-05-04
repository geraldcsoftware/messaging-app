using MessagingApp.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.Infrastructure.Models;

[EntityTypeConfiguration(typeof(ClientEntityTypeConfiguration))]
public class Client
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}
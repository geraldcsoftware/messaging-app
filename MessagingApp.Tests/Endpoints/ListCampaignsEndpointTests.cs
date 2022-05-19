using System.Net;
using System.Text.Json;
using Bogus;
using FluentAssertions;
using MessagingApp.Infrastructure;
using MessagingApp.Infrastructure.Models;
using MessagingApp.Tests.Utilities;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace MessagingApp.Tests.Endpoints;

public class ListCampaignsEndpointTests : IClassFixture<ApplicationFactory>
{
    private readonly ApplicationFactory _factory;


    public ListCampaignsEndpointTests(ApplicationFactory factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task ListCampaigns_WhenCampaignsExists_ReturnsOkWithCampaignDetails()
    {
        // Arrange
        var clientId = Guid.NewGuid().ToString();

        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                using var scope = services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
                var client = new Faker<Client>()
                            .RuleFor(x => x.Id, clientId)
                            .RuleFor(x => x.Name, f => f.Person.Company.Name)
                            .RuleFor(x => x.Email, f => f.Person.Email)
                            .Generate();
                var campaigns = new Faker<MessageCampaign>()
                               .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
                               .RuleFor(x => x.Name, f => f.Lorem.Word())
                               .RuleFor(x => x.Template, f => f.Lorem.Sentence())
                               .RuleFor(x => x.Created, f => f.Date.Recent())
                               .RuleFor(x => x.ClientId, clientId)
                               .Generate(50);

                context.Clients.Add(client);
                context.Campaigns.AddRange(campaigns);
                context.SaveChanges();
            });
        });

        var client = factory.CreateClient();
        var url = $"/api/clients/{clientId}/campaigns?page=1&pageSize=10";

        // Act
        var response = await client.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseContent.Should().NotBeNullOrEmpty();
        var responseJson = JsonDocument.Parse(responseContent);

        responseJson.RootElement.GetProperty("totalItems").GetInt32().Should().Be(50);
        responseJson.RootElement.GetProperty("items").GetArrayLength().Should().Be(10);
    }
}
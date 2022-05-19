using Mapster;
using MapsterMapper;
using MessagingApp.Api.ViewModels;
using MessagingApp.Infrastructure.Models;

namespace MessagingApp.Api.Mapping;

public static class MapperRegistrationExtensions
{
   public static void AddMapper(this IServiceCollection services)
    {
        var config = new TypeAdapterConfig();
        config.ForType<MessageCampaign, CampaignViewModel>()
              .Map(d => d.Created, src => DateTime.SpecifyKind(src.Created, DateTimeKind.Utc))
              .Map(d => d.MessageTemplate, src => src.Template);
        
        services.AddSingleton(config);
        services.AddTransient<IMapper, ServiceMapper>();
    }
}
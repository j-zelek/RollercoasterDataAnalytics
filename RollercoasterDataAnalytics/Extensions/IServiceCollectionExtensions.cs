using RollercoasterDataAnalytics.Configurations;
using RollercoasterDataAnalytics.Json;
using RollercoasterDataAnalytics.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RollercoasterDataAnalytics.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonOptions(this IServiceCollection services)
        {
            services.Configure<JsonSerializerOptions>(jsonOptions =>
            {
                jsonOptions.PropertyNameCaseInsensitive = true;
                jsonOptions.Converters.Add(new DateOnlyConverter());
                jsonOptions.Converters.Add(new TimeOnlyConverter());
                jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

            return services;
        }

        public static IServiceCollection AddWartezeitenAppClient(this IServiceCollection services, IConfiguration configuration) {
            
            var sectionPrefix = "WartezeitenApp";
            var baseUri = configuration.GetValue<string>($"{sectionPrefix}:Uri")!;

            services.Configure<WartezeitenAppConfiguration>(o =>
            {
                o.Uri = baseUri;
                o.Language = configuration.GetValue<string>($"{sectionPrefix}:Language", "en")!; // compiler still says this can be null despite the default value
                o.ApiVersion = configuration.GetValue<string>($"{sectionPrefix}:ApiVersion", "v1")!;
            });
            services.AddHttpClient(Constants.WARTEZEITEN_APP_CLIENT_NAME, c =>
            {
                c.BaseAddress = new Uri(baseUri);
            });
            services.AddTransient<IWartezeitenAppService, WartezeitenAppService>();

            return services;
        }
    }
}

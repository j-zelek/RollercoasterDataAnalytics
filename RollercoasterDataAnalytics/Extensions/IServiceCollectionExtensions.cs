using RollercoasterDataAnalytics.Configurations;
using RollercoasterDataAnalytics.Services;
using System.Text.Json;

namespace RollercoasterDataAnalytics.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddJsonOptions(this IServiceCollection services)
        {
            services.Configure<JsonSerializerOptions>(jsonOptions =>
            {
                jsonOptions.PropertyNameCaseInsensitive = true;
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

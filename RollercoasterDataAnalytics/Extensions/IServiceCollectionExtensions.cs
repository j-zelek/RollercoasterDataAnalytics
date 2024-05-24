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
            services.Configure<WartezeitenAppConfiguration>(o =>
            {
                var sectionPrefix = "WartezeitenApp";
                o.Uri = configuration.GetValue<string>($"{sectionPrefix}:Uri")!;
                o.Language = configuration.GetValue<string>($"{sectionPrefix}:Language", "en")!; // compiler still says this can be null despite the default value
                o.ApiVersion = configuration.GetValue<string>($"{sectionPrefix}:ApiVersion", "v1")!;
            });
            services.AddTransient<IWartezeitenAppService, WartezeitenAppService>();

            return services;
        }
    }
}

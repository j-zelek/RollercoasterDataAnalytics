using Microsoft.Extensions.Options;
using RestSharp;
using RollercoasterDataAnalytics.Configurations;
using RollercoasterDataAnalytics.Models;

namespace RollercoasterDataAnalytics.Services;

public interface IWartezeitenAppService
{
    Task<IEnumerable<Park>> GetParks();
}

public class WartezeitenAppService : IWartezeitenAppService
{
    private readonly ILogger<WartezeitenAppService> _logger;
    private readonly WartezeitenAppConfiguration _configuration;
    private readonly IRestClient _restClient;

    private const string LANGUAGE_HEADER = "language";

    public WartezeitenAppService(ILogger<WartezeitenAppService> logger, IOptions<WartezeitenAppConfiguration> wartezeitenAppConfiguration)
    {
        _logger = logger;
        _configuration = wartezeitenAppConfiguration.Value ?? throw new ArgumentNullException($"Failed to load configuration for {typeof(WartezeitenAppConfiguration).Name}");
        _restClient = new RestClient(_configuration.Uri);
        _restClient.AddDefaultHeader(LANGUAGE_HEADER, _configuration.Language);
     }
}
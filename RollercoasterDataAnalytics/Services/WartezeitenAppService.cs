using System.Text.Json;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Serializers.Json;
using RollercoasterDataAnalytics.Configurations;
using RollercoasterDataAnalytics.Models;

namespace RollercoasterDataAnalytics.Services;

public interface IWartezeitenAppService
{
    /// <summary>
    /// Fetches all parks. The endpoint caches reponses for 24 hours.
    /// Detailed documentation can be found here: https://api.wartezeiten.app/#/default/get_v1_parks
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Park>?> GetParksAsync();
    /// <summary>
    /// Fetches all waiting times for the given park. The endpoint caches reponses for 5 minutes.
    /// Detailed documentation can be found here: https://api.wartezeiten.app/#/default/get_v1_waitingtimes
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<WaitingTime>?> GetWaitingTimesAsync(Park park) => GetWaitingTimesAsync(park.Id);
    /// <summary>
    /// Fetches all waiting times for the given park. The endpoint caches reponses for 5 minutes.
    /// Detailed documentation can be found here: https://api.wartezeiten.app/#/default/get_v1_waitingtimes
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<WaitingTime>?> GetWaitingTimesAsync(string parkId);
}

public class WartezeitenAppService : IWartezeitenAppService
{
    private readonly ILogger<WartezeitenAppService> _logger;
    private readonly IRestClient _restClient;
    private readonly WartezeitenAppConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string LANGUAGE_HEADER = "language";
    private const string WAITING_TIMES_QUERY_HEADER = "park";
    private const string PARKS_ENDPOINT = "parks";
    private const string WAITING_TIMES_ENDPOINT = "waitingtimes";

    public WartezeitenAppService(ILogger<WartezeitenAppService> logger, IOptions<WartezeitenAppConfiguration> wartezeitenAppConfiguration, IHttpClientFactory clientFactory,IOptions<JsonSerializerOptions> jsonSerializerOptions)
    {
        _logger = logger;
        _configuration = wartezeitenAppConfiguration.Value ?? throw new ArgumentNullException($"Failed to load configuration for {typeof(WartezeitenAppConfiguration).Name}");
        _jsonSerializerOptions = jsonSerializerOptions.Value ?? throw new ArgumentNullException($"Failed to load configuration for {typeof(JsonSerializerOptions).Name}");

        _restClient = new RestClient(clientFactory.CreateClient(Constants.WARTEZEITEN_APP_CLIENT_NAME), configureSerialization: s => { s.UseSystemTextJson(_jsonSerializerOptions); });
        _restClient.AddDefaultHeader(LANGUAGE_HEADER, _configuration.Language);
    }

    public async Task<IEnumerable<Park>?> GetParksAsync()
    {
        var request = new RestRequest($"{_configuration.ApiVersion}/{PARKS_ENDPOINT}");
        try
        {
            _logger.LogDebug("Fetching Parks");
            var response = await _restClient.ExecuteAsync<IEnumerable<Park>>(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return response.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch parks");
        }

        return null;
    }

    public async Task<IEnumerable<WaitingTime>?> GetWaitingTimesAsync(string parkId)
    {
        var request = new RestRequest($"{_configuration.ApiVersion}/{WAITING_TIMES_ENDPOINT}");
        request.AddHeader(WAITING_TIMES_QUERY_HEADER, parkId);
        try
        {
            _logger.LogDebug("Requesting waiting times for park {0}", parkId);
            var response = await _restClient.ExecuteAsync<IEnumerable<WaitingTime>>(request).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return response.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch waiting times for park {0}", parkId);
        }

        return null;
    }
}
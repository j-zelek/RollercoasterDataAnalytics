using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using RollercoasterDataAnalytics.Configurations;
using RollercoasterDataAnalytics.Services;
using RollercoasterDataAnalytics.Models;
using RollercoasterDataAnalytics.Json;

namespace RollercoasterDataAnalytics.Tests
{
    public class WartezeitenAppServiceTests
    {
        private readonly MockHttpMessageHandler _messageHandler;
        private readonly Mock<ILogger<WartezeitenAppService>> _mockLogger;
        private readonly Mock<IHttpClientFactory> _mockFactory;
        private readonly WartezeitenAppConfiguration _config;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string TEST_URI = "http://wartezeiten.test";

        public WartezeitenAppServiceTests()
        {
            _messageHandler = new MockHttpMessageHandler();

            _mockLogger = new Mock<ILogger<WartezeitenAppService>>();
            _mockFactory = new Mock<IHttpClientFactory>();

            _config = new WartezeitenAppConfiguration
            {
                Uri = TEST_URI,
                ApiVersion = "v1",
                Language = "en"
            };
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new DateOnlyConverter());
            _jsonOptions.Converters.Add(new TimeOnlyConverter());
            _jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }

        [Fact]
        public async void GetParksAsync_Succeeds()
        {
            SetupClient($"{TEST_URI}/v1/parks", HttpStatusCode.OK, "parks");

            var service = new WartezeitenAppService(_mockLogger.Object, Options.Create(_config), _mockFactory.Object, Options.Create(_jsonOptions));
            var list = await service.GetParksAsync();

            Assert.NotNull(list);
            Assert.IsType<Park>(list.First());
        }

        [Fact]
        public async void GetParksAsync_Fails()
        {
            SetupClient($"{TEST_URI}/v1/parks", HttpStatusCode.NotFound);

            var service = new WartezeitenAppService(_mockLogger.Object, Options.Create(_config), _mockFactory.Object, Options.Create(_jsonOptions));
            var list = await service.GetParksAsync();

            Assert.Null(list);
        }

        [Fact]
        public async void GetWaitingTimes_Succeeds()
        {
            SetupClient($"{TEST_URI}/v1/waitingtimes", HttpStatusCode.OK, "waitingtimes");

            var service = new WartezeitenAppService(_mockLogger.Object, Options.Create(_config), _mockFactory.Object, Options.Create(_jsonOptions));
            var list = await service.GetWaitingTimesAsync("somePark");

            Assert.NotNull(list);
            Assert.IsType<WaitingTime>(list.First());
        }

        [Fact]
        public async void GetWaitingTimes_Fails()
        {
            SetupClient($"{TEST_URI}/v1/waitingtimes", HttpStatusCode.NotFound);

            var service = new WartezeitenAppService(_mockLogger.Object, Options.Create(_config), _mockFactory.Object, Options.Create(_jsonOptions));
            var list = await service.GetWaitingTimesAsync("somePark");

            Assert.Null(list);
        }

        private static string GetMockResponse(string? key)
        {
            if (key is null)
                return string.Empty;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "data/wartezeit.json");
            using var reader = new StreamReader(path);
            var content = reader.ReadToEnd();
            var json = JsonDocument.Parse(content);
            return json.RootElement.EnumerateObject().First(n => n.Name == key).Value.ToString();
        }

        private void SetupClient(string uri, HttpStatusCode statusCode, string? responseKey = null)
        {
            _messageHandler.When(uri).WithHeaders(new Dictionary<string, string> { { "language", "en" } }).Respond(statusCode, "application/json", GetMockResponse(responseKey));
            var client = _messageHandler.ToHttpClient();
            client.BaseAddress = new Uri(TEST_URI);
            _mockFactory.Setup(f => f.CreateClient(Constants.WARTEZEITEN_APP_CLIENT_NAME)).Returns(client);
        }
    }
}

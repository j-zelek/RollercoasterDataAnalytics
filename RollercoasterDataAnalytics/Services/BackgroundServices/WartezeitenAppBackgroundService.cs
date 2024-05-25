namespace RollercoasterDataAnalytics.Services.BackgroundServices
{
    internal class WartezeitenAppBackgroundService : BackgroundService
    {
        private readonly ILogger<WartezeitenAppBackgroundService> _logger;
        private readonly IWartezeitenAppService _wartezeitenAppService;

        public WartezeitenAppBackgroundService(ILogger<WartezeitenAppBackgroundService> logger, IWartezeitenAppService wartezeitenAppService)
        {
            _logger = logger;
            _wartezeitenAppService = wartezeitenAppService;
        }

        private async Task LoadParksAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Loading parks");
                var parks = await _wartezeitenAppService.GetParksAsync().ConfigureAwait(false);
                _logger.LogInformation("Loaded {0} parks", parks?.Count());

                if (parks?.Any() ?? false)
                {
                    // TODO: persist loaded data
                }

                // The parks endpoint caches the responses for 24 hours. I hardcoded this for simplicity because I don't expect this value to change.
                // Might still set the time in the configuration file in the long run.
                _logger.LogInformation("Finished background processing of parks. Waiting 24 hours for new run");
                await Task.Delay(TimeSpan.FromHours(24), cancellationToken);
            }
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) { 
                await LoadParksAsync(stoppingToken).ConfigureAwait(false);
            }
        }
    }
}

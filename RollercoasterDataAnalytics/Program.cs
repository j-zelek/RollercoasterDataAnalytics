using RollercoasterDataAnalytics.Extensions;

var builder = Host.CreateApplicationBuilder(args);

var config = builder.Configuration;

var services = builder.Services;
services
    .AddJsonOptions()
    .AddWartezeitenAppClient(config);

var host = builder.Build();
host.Run();

// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma warning disable SA1600

using Aksio.Cratis.Client;
using Aksio.Cratis.Kernel.Grains.Clients;

namespace Aksio.Cratis.Kernel.Server;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMongoDBReadModels();
        services.AddHttpClient(ConnectedClients.ConnectedClientsHttpClient).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
#pragma warning disable MA0039 // Allowing self-signed certificates for clients connecting to the Kernel
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();

        app.UseRouting();
        app.UseWebSockets();
        app.UseCratis(false);
        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        appLifetime.ApplicationStarted.Register(() =>
        {
            logger.PerformingBootProcedures();
            app.PerformBootProcedures();

            KernelReadyResourceFilter.KernelReady = true;
            app.ApplicationServices.GetRequiredService<IClient>().Connect().Wait();
        });
        app.UseAksio();
    }
}

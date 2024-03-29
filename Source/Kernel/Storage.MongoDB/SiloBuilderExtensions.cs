// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis;
using Aksio.Cratis.Compliance.MongoDB;
using Aksio.Cratis.Kernel.Storage;
using Aksio.Cratis.Kernel.Storage.Compliance;
using Aksio.Cratis.Kernel.Storage.MongoDB;
using Aksio.Cratis.Kernel.Storage.MongoDB.Reminders;
using Aksio.Cratis.Kernel.Storage.MongoDB.Tenants;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Orleans.Runtime;
using Orleans.Storage;

namespace Orleans.Hosting;

/// <summary>
/// Extension methods for <see cref="ISiloBuilder"/> for configuring event sequence stream.
/// </summary>
public static class SiloBuilderExtensions
{
    /// <summary>
    /// Add event sequence stream support.
    /// </summary>
    /// <param name="builder"><see cref="ISiloBuilder"/> to add for.</param>
    /// <returns><see cref="ISiloBuilder"/> for builder continuation.</returns>
    public static ISiloBuilder UseMongoDB(this ISiloBuilder builder)
    {
        // TODO: Store Grain state in Mongo
        builder.AddMemoryGrainStorage("PubSubStore");
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IDatabase, Database>();
            services.AddSingleton<IMongoDBClientManager, MongoDBClientManager>();
            services.AddSingleton<IEncryptionKeyStorage, EncryptionKeyStorage>();
            services.AddSingleton<IStorage, Aksio.Cratis.Kernel.Storage.MongoDB.Storage>();
            services.AddSingletonNamedService<IGrainStorage>(WellKnownGrainStorageProviders.TenantConfiguration, (serviceProvider, _) => serviceProvider.GetRequiredService<TenantConfigurationStorageProvider>());
        });

        BsonSerializer.RegisterSerializer(new JsonElementSerializer());
        BsonSerializer.RegisterSerializer(new UriSerializer());

        builder.AddReminders();
        builder.ConfigureServices(services => services.AddSingleton<IReminderTable, ReminderTable>());
        return builder;
    }
}

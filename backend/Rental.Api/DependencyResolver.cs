﻿using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Nest.JsonNetSerializer;
using Rental.Domain.Repositories;
using Rental.Infrastructure.Repositories;
using System;

namespace Rental.Api
{
    public static class DependencyResolver
    {
        public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var uri = configuration.GetSection("ELASTICSEARCH_URL")?.Value;
            var connectionSettings = new ConnectionSettings(
                new SingleNodeConnectionPool(new Uri(uri)),
                JsonNetSerializer.Default
                )
                .TransferEncodingChunked();

            services.AddSingleton(provider => new ElasticLowLevelClient(connectionSettings));
            services.AddSingleton(provider => new ElasticClient(connectionSettings));
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IEmployeesRepository, EmployeesRepository>();
            return services;
        }

        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            return services;
        }
    }
}
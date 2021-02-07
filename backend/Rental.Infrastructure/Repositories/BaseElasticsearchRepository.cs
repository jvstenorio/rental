using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rental.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Infrastructure.Repositories
{
    public class BaseElasticsearchRepository<T> where T : new()
    {
        private readonly ILogger<BaseElasticsearchRepository<T>> _logger;
        private readonly IMemoryCacheRepository _memoryCacheRepository;
        private readonly ElasticLowLevelClient _elasticLowLevelClient;
        private readonly ElasticClient _client;
        private static readonly SnakeCaseNamingStrategy _snakeCase = new SnakeCaseNamingStrategy();
        public readonly string _index;

        public BaseElasticsearchRepository(
            IServiceProvider provider,
            ILogger<BaseElasticsearchRepository<T>> logger,
            IMemoryCacheRepository memoryCacheRepository)
        {
            _logger = logger;
            _memoryCacheRepository = memoryCacheRepository;
            _elasticLowLevelClient = provider.GetService<ElasticLowLevelClient>();
            _client = provider.GetService<ElasticClient>();
            _index = _snakeCase.GetPropertyName(typeof(T).Name, false);
        }

        public async Task AddAsync(object entity, CancellationToken cancellationToken)
        {
            var response = await _client.IndexAsync(entity, i => i.Index(_index), cancellationToken);
        }
        public async Task<T> GetByIdentifierAsync(Guid identifier, CancellationToken cancellationToken) =>
            await _memoryCacheRepository.GetOrCreateAsync(
                identifier.ToString(),
                () => GetByIdentifierQueryAsync(identifier, cancellationToken)
                );

        public async Task<List<T>> ListAllAsync(CancellationToken cancellationToken) =>
            await _memoryCacheRepository.GetOrCreateAsync(
                typeof(List<T>).Name,
                () => ListAllQueryAsync(cancellationToken)
                );

        public async Task<T> GetByIdentifierQueryAsync(Guid identifier, CancellationToken cancellationToken)
        {
            var query = $"{{\"query\":{{\"term\":{{\"identifier.keyword\":\"{identifier}\"}}}}}}}}";

            var searchResponse = await _client.SearchAsync<object>(search => search
                                        .Index(_index)
                                        .Query(q => q.SimpleQueryString(s => s.Query(query)))
                                        , cancellationToken);

            var document = searchResponse.Documents.FirstOrDefault();

            return document != null ? DeserializeEntityFromDocument(document) : default;

        }
        private async Task<List<T>> ListAllQueryAsync(CancellationToken cancellationToken)
        {

            var searchResponse = await _client.SearchAsync<object>(search => search
                                        .Index(_index)
                                        .Size(10000)
                                        .Query(q => q.MatchAll())
                                        , cancellationToken);

            var entities = searchResponse.Documents?.Select(d => DeserializeEntityFromDocument(d))?.ToList();

            return entities != null ? entities : new List<T>();
        }

        private T DeserializeEntityFromDocument(object document)
        {
            var serializedResult = JsonConvert.SerializeObject(document);
            return JsonConvert.DeserializeObject<T>(serializedResult);
        }
    }
}

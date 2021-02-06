using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rental.Infrastructure.Repositories
{
    public class BaseElasticsearchRepository<T>
    {
        private readonly ILogger<BaseElasticsearchRepository<T>> _logger;
        private readonly ElasticLowLevelClient _elasticLowLevelClient;
        private readonly ElasticClient _client;
        private readonly string _index;
        private static readonly SnakeCaseNamingStrategy _snakeCase = new SnakeCaseNamingStrategy();

        public BaseElasticsearchRepository(IServiceProvider provider, ILogger<BaseElasticsearchRepository<T>> logger)
        {
            _logger = logger;
            _elasticLowLevelClient = provider.GetService<ElasticLowLevelClient>();
            _client = provider.GetService<ElasticClient>();
            _index = _snakeCase.GetPropertyName(typeof(T).Name, false);
        }

        public async Task AddAsync(object entity, CancellationToken cancellationToken)
        {
            var response = await _client.IndexAsync(entity, i => i.Index(_index));
        }
        public async Task<T> GetByIdentifierAsync(Guid identifier, CancellationToken cancellationToken)
        {
            var query = $"{{\"query\":{{\"term\":{{\"transactionId.keyword\":\"{identifier}\"}}}}}}}}";

            var searchResponse = await _client.SearchAsync<object>(search => search
                                        .Index(_index)
                                        .Query(qry => qry.SimpleQueryString(s => s.Query(query)))
                                        , cancellationToken);

            var document = searchResponse.Documents.FirstOrDefault();

            return document != null ? DeserializeEntityFromDocument(document) : default;

        }

        public async Task<List<T>> ListAllAsync(CancellationToken cancellationToken)
        {

            var searchResponse = await _client.SearchAsync<object>(search => search
                                        .Index(_index)
                                        .Size(10000)
                                        .Query(q => q.MatchAll())
                                        , cancellationToken);

            var entities = searchResponse.Documents?.Select( d => DeserializeEntityFromDocument(d))?.ToList();

            return entities != null ? entities : new List<T>();
        }

        private T DeserializeEntityFromDocument(object document)
        {
            var serializedResult = JsonConvert.SerializeObject(document);
            return JsonConvert.DeserializeObject<T>(serializedResult);
        }
    }
}

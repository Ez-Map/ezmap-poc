using Elasticsearch.Net;
using Nest;

namespace EzMap.Api.Services;

public interface IElasticSearchService<T> where T : class
{
    Task CreateIndexIfNotExists(string indexName);
    Task<bool> AddOrUpdateBulk(IEnumerable<T> documents);
    Task<bool> AddOrUpdate(T document);
    Task<T> Get(string key);
    Task<List<T>?> GetAll();
    Task<List<T>?> Query(QueryContainer predicate);
    Task<bool> Remove(string key);
    Task<long> RemoveAll();
}

public class ElasticSearchService<T> : IElasticSearchService<T> where T : class
{
    private string _indexName { get; set; }
    private readonly IElasticClient _client;

    public ElasticSearchService(IElasticClient client)
    {
        _client = client;
    }

    public ElasticSearchService<T> Index(string indexName)
    {
        _indexName = indexName;
        return this;
    }
    
    public async Task CreateIndexIfNotExists(string indexName)
    {
        if (!_client.Indices.Exists(indexName).Exists)
        {
            await _client.Indices.CreateAsync(indexName, c => c .Map<T>(m => m.AutoMap()));
        }

        Index(indexName);
    }

    public async Task<bool> AddOrUpdateBulk(IEnumerable<T> documents)
    {
        var indexResponse = await _client.BulkAsync(b => b
            .Index(_indexName)
            .UpdateMany(documents, (ud, d) => ud.Doc(d).DocAsUpsert(true))
        );
        return indexResponse.IsValid;
    }

    public async Task<bool> AddOrUpdate(T document)
    {
        var indexResponse = await _client.IndexAsync(document, idx => idx.Index(_indexName).OpType(OpType.Index));
        return indexResponse.IsValid;
    }

    public async Task<T> Get(string key)
    {
        var response = await _client.GetAsync<T>(key, g => g.Index(_indexName));
        return response.Source;
    }

    public async Task<List<T>?> GetAll()
    {
        var searchResponse = await _client.SearchAsync<T>(s => s.Index(_indexName).Query(q => q.MatchAll()));
        return searchResponse.IsValid ? searchResponse.Documents.ToList() : default;
    }

    public async Task<List<T>?> Query(QueryContainer predicate)
    {
        var searchResponse = await _client.SearchAsync<T>(s => s.Index(_indexName).Query(q => predicate));
        return searchResponse.IsValid ? searchResponse.Documents.ToList() : default;
    }

    public async Task<bool> Remove(string key) 
    {
        var response = await _client.DeleteAsync<T>(key, g => g.Index(_indexName));
        return response.IsValid;
    }

    public async Task<long> RemoveAll()
    {
        var response = await _client.DeleteByQueryAsync<T>(q => q.Index(_indexName));
        return response.Deleted;
    }
}
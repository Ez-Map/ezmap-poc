using Elasticsearch.Net;
using Nest;

namespace EzMap.Api.Services;

public interface IElasticSearchService
{
    Task CreateIndexIfNotExists(string indexName);
    Task<bool> AddOrUpdateBulk(IEnumerable<object> documents);
    Task<bool> AddOrUpdate(object document);
    Task<object> Get(string key);
    Task<List<object>?> GetAll();
    Task<List<object>?> Query(QueryContainer predicate);
    Task<bool> Remove(string key);
    Task<long> RemoveAll();
}

public class ElasticSearchService : IElasticSearchService
{
    private string _indexName { get; set; }
    private readonly IElasticClient _client;

    public ElasticSearchService(IElasticClient client)
    {
        _indexName = "thanh1";
        _client = client;
    }

    public ElasticSearchService SetIndex(string indexName)
    {
        _indexName = indexName;
        return this;
    }
    
    public async Task CreateIndexIfNotExists(string indexName)
    {
        if (!_client.Indices.Exists(indexName).Exists)
        {
            await _client.Indices.CreateAsync(indexName, c => c .Map<object>(m => m.AutoMap()));
        }

        SetIndex(indexName);
    }

    public async Task<bool> AddOrUpdateBulk(IEnumerable<object> documents)
    {
        var indexResponse = await _client.BulkAsync(b => b
            .Index(_indexName)
            .UpdateMany(documents, (ud, d) => ud.Doc(d).DocAsUpsert(true))
        );
        return indexResponse.IsValid;
    }

    public async Task<bool> AddOrUpdate(object document)
    {
        var indexResponse = await _client.IndexAsync(document, idx => idx.Index(_indexName).OpType(OpType.Index));
        return indexResponse.IsValid;
    }

    public async Task<object> Get(string key)
    {
        var response = await _client.GetAsync<object>(key, g => g.Index(_indexName));
        return response.Source;
    }

    public async Task<List<object>?> GetAll()
    {
        var searchResponse = await _client.SearchAsync<object>(s => s.Index(_indexName).Query(q => q.MatchAll()));
        return searchResponse.IsValid ? searchResponse.Documents.ToList() : default;
    }

    public async Task<List<object>?> Query(QueryContainer predicate)
    {
        var searchResponse = await _client.SearchAsync<object>(s => s.Index(_indexName).Query(q => predicate));
        return searchResponse.IsValid ? searchResponse.Documents.ToList() : default;
    }

    public async Task<bool> Remove(string key) 
    {
        var response = await _client.DeleteAsync<object>(key, g => g.Index(_indexName));
        return response.IsValid;
    }

    public async Task<long> RemoveAll()
    {
        var response = await _client.DeleteByQueryAsync<object>(q => q.Index(_indexName));
        return response.Deleted;
    }
}
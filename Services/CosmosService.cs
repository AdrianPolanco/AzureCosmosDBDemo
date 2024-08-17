using AzureCosmosDBDemo.Interfaces;
using AzureCosmosDBDemo.Models;
using AzureCosmosDBDemo.Models.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AzureCosmosDBDemo.Services
{
    public class CosmosService : ICosmosService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CosmosService(CosmosClient cosmosClient, IOptions<CosmosOptions> options)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(options.Value.DatabaseName, options.Value.ContainerName);
        }

        public async Task<Item> AddAsync(Item item)
        {
            item.Id = Guid.NewGuid().ToString();
            item.Rid = item.Id.ToString();
            item.Ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ItemResponse<Item>? response = await _container.CreateItemAsync(item, new PartitionKey(item.Id));
            return response.Resource;
        }

        public async Task<Item> DeleteAsync(string id)
        {
            var response = await _container.DeleteItemAsync<Item>(id, new PartitionKey(id));
            return response.Resource;
        }

        public async Task<Item?> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Item>(id, new PartitionKey(id));
                return response.Resource;
            }catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Item>> GetMultipleAsync(string queryString)
        {
            /*var _query = _container.GetItemLinqQueryable<Item>(allowSynchronousQueryExecution: true).Where(i => i.Type == "Item")
                .ToFeedIterator();*/

            var query = _container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
            List<Item> results = new List<Item>();

            while(query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Item> UpdateAsync(string id, Item item)
        {
            item.Ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            item.Id = id;
            var response = await _container.UpsertItemAsync(item, new PartitionKey(id));
            return response.Resource;
        }
    }
}

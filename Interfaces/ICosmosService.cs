using AzureCosmosDBDemo.Models;
using Microsoft.Azure.Cosmos;

namespace AzureCosmosDBDemo.Interfaces
{
    public interface ICosmosService
    {
        Task<IEnumerable<Item>> GetMultipleAsync(string query);
        Task<Item?> GetAsync(string id);
        Task<Item> AddAsync(Item item);
        Task<Item> UpdateAsync(string id, Item item);
        Task<Item> DeleteAsync(string id);
    }
}


using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.api.Data
{
    // Implement Decorator pattern by adding caching service to basketRepository
    public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
    {
        

        public async Task<ShoppingCart> GetBasketAsync(string userName, CancellationToken cancellationToken = default)
        {
            // Check first in cache
            var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
            if (!string.IsNullOrEmpty(cachedBasket))
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;

            var basket = await repository.GetBasketAsync(userName, cancellationToken);
            // Add it to the cache
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);
            return basket;
        }

        public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
        {
            var basket = await repository.StoreBasketAsync(shoppingCart, cancellationToken);
            // Add it to the cache
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);

            return basket;
        }

        public async Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
        {
            await repository.DeleteBasketAsync(userName, cancellationToken);

            await cache.RemoveAsync(userName, cancellationToken);

            return true;

        }
    }
}

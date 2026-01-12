using ProductCatalogAPI.Caching;
using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly RedisCacheService _cacheService;

        private static readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 50000 },
            new Product { Id = 2, Name = "Mobile", Price = 20000 }
        };

        public ProductService(RedisCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            const string cacheKey = "product_list";

            var cachedProducts = await _cacheService.GetAsync<List<Product>>(cacheKey);
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            await _cacheService.SetAsync(cacheKey, _products, TimeSpan.FromMinutes(5));
            return _products;
        }
    }

}

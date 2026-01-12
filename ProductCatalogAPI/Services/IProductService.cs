using ProductCatalogAPI.Models;

namespace ProductCatalogAPI.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();

    }
}

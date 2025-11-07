using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(int? categoryId, int pageNumber, int pageSize);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
}
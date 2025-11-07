using CatalogService.Application.DTOs;

namespace CatalogService.Application.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(int? categoryId, int pageNumber, int pageSize);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto productDto);
    Task UpdateProductAsync(int id, UpdateProductDto productDto);
    Task DeleteProductAsync(int id);
}
using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CatalogService.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateProduct_WithValidData_ShouldSucceed()
        {
            var productRepo = new InMemoryProductRepository();
            var categoryRepo = new InMemoryCategoryRepository();
            var service = new ProductService(productRepo, categoryRepo);

            var category = await categoryRepo.AddAsync(new Category { Id = 1, Name = "Electronics" });

            var dto = new CreateProductDto
            {
                Name = "Laptop",
                Description = "High-end laptop",
                CategoryId = 1,
                Price = 1299.99m,
                Amount = 10
            };

            var result = await service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Laptop", result.Name);
            Assert.Equal(1299.99m, result.Price);
        }

        [Fact]
        public async Task CreateProduct_WithInvalidCategory_ShouldThrowException()
        {
            var productRepo = new InMemoryProductRepository();
            var categoryRepo = new InMemoryCategoryRepository();
            var service = new ProductService(productRepo, categoryRepo);

            var dto = new CreateProductDto
            {
                Name = "Laptop",
                CategoryId = 999,
                Price = 1299.99m,
                Amount = 10
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateProduct_WithNegativePrice_ShouldThrowException()
        {
            var productRepo = new InMemoryProductRepository();
            var categoryRepo = new InMemoryCategoryRepository();
            var service = new ProductService(productRepo, categoryRepo);

            await categoryRepo.AddAsync(new Category { Id = 1, Name = "Electronics" });

            var dto = new CreateProductDto
            {
                Name = "Laptop",
                CategoryId = 1,
                Price = -100m,
                Amount = 10
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
        }

        [Fact]
        public async Task GetByCategoryId_ShouldReturnOnlyProductsFromCategory()
        {
            var productRepo = new InMemoryProductRepository();
            var categoryRepo = new InMemoryCategoryRepository();
            var service = new ProductService(productRepo, categoryRepo);

            var cat1 = await categoryRepo.AddAsync(new Category { Id = 1, Name = "Electronics" });
            var cat2 = await categoryRepo.AddAsync(new Category { Id = 2, Name = "Books" });

            await productRepo.AddAsync(new Product { Id = 1, Name = "Laptop", CategoryId = 1, Price = 1000m, Amount = 5, Category = cat1 });
            await productRepo.AddAsync(new Product { Id = 2, Name = "Phone", CategoryId = 1, Price = 500m, Amount = 10, Category = cat1 });
            await productRepo.AddAsync(new Product { Id = 3, Name = "Novel", CategoryId = 2, Price = 15m, Amount = 20, Category = cat2 });

            var results = await service.GetByCategoryIdAsync(1);

            Assert.Equal(2, results.Count());
            Assert.All(results, p => Assert.Equal(1, p.CategoryId));
        }
    }

    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();
        private int _nextId = 1;

        public Task<Product> GetByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Product>>(_products);
        }

        public Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return Task.FromResult<IEnumerable<Product>>(_products.Where(p => p.CategoryId == categoryId));
        }

        public Task<Product> AddAsync(Product product)
        {
            if (product.Id == 0)
                product.Id = _nextId++;
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<Product> UpdateAsync(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing != null)
            {
                _products.Remove(existing);
                _products.Add(product);
            }
            return Task.FromResult(product);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> ExistsAsync(int id)
        {
            return Task.FromResult(_products.Any(p => p.Id == id));
        }
    }

    public class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories = new();
        private int _nextId = 1;

        public Task<Category> GetByIdAsync(int id)
        {
            return Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Category>>(_categories);
        }

        public Task<Category> AddAsync(Category category)
        {
            if (category.Id == 0)
                category.Id = _nextId++;
            _categories.Add(category);
            return Task.FromResult(category);
        }

        public Task<Category> UpdateAsync(Category category)
        {
            var existing = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existing != null)
            {
                _categories.Remove(existing);
                _categories.Add(category);
            }
            return Task.FromResult(category);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _categories.Remove(category);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> ExistsAsync(int id)
        {
            return Task.FromResult(_categories.Any(c => c.Id == id));
        }
    }
}

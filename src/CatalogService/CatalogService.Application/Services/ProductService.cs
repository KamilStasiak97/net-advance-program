using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryIdAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryIdAsync(categoryId);
            return products.Select(MapToDto);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            await ValidateCreateDtoAsync(dto);

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Image = dto.Image,
                CategoryId = dto.CategoryId,
                Price = dto.Price,
                Amount = dto.Amount
            };

            var created = await _productRepository.AddAsync(product);
            return MapToDto(created);
        }

        public async Task<ProductDto> UpdateAsync(UpdateProductDto dto)
        {
            await ValidateUpdateDtoAsync(dto);

            var existing = await _productRepository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                throw new InvalidOperationException($"Product with ID {dto.Id} not found.");
            }

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Image = dto.Image;
            existing.CategoryId = dto.CategoryId;
            existing.Price = dto.Price;
            existing.Amount = dto.Amount;

            var updated = await _productRepository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (!await _productRepository.ExistsAsync(id))
            {
                throw new InvalidOperationException($"Product with ID {id} not found.");
            }

            return await _productRepository.DeleteAsync(id);
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Price = product.Price,
                Amount = product.Amount
            };
        }

        private async Task ValidateCreateDtoAsync(CreateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Product name is required.", nameof(dto.Name));

            if (dto.Name.Length > 50)
                throw new ArgumentException("Product name cannot exceed 50 characters.", nameof(dto.Name));

            if (dto.CategoryId <= 0)
                throw new ArgumentException("Valid category ID is required.", nameof(dto.CategoryId));

            if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
                throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist.");

            if (dto.Price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(dto.Price));

            if (dto.Amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(dto.Amount));
        }

        private async Task ValidateUpdateDtoAsync(UpdateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Id <= 0)
                throw new ArgumentException("Invalid product ID.", nameof(dto.Id));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Product name is required.", nameof(dto.Name));

            if (dto.Name.Length > 50)
                throw new ArgumentException("Product name cannot exceed 50 characters.", nameof(dto.Name));

            if (dto.CategoryId <= 0)
                throw new ArgumentException("Valid category ID is required.", nameof(dto.CategoryId));

            if (!await _categoryRepository.ExistsAsync(dto.CategoryId))
                throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist.");

            if (dto.Price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(dto.Price));

            if (dto.Amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(dto.Amount));
        }
    }
}

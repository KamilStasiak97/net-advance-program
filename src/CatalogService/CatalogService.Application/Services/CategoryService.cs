using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return category == null ? null : MapToDto(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            ValidateCreateDto(dto);

            if (dto.ParentCategoryId.HasValue && !await _repository.ExistsAsync(dto.ParentCategoryId.Value))
            {
                throw new InvalidOperationException($"Parent category with ID {dto.ParentCategoryId.Value} does not exist.");
            }

            var category = new Category
            {
                Name = dto.Name,
                Image = dto.Image,
                ParentCategoryId = dto.ParentCategoryId
            };

            var created = await _repository.AddAsync(category);
            return MapToDto(created);
        }

        public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto)
        {
            ValidateUpdateDto(dto);

            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing == null)
            {
                throw new InvalidOperationException($"Category with ID {dto.Id} not found.");
            }

            if (dto.ParentCategoryId.HasValue)
            {
                if (dto.ParentCategoryId.Value == dto.Id)
                {
                    throw new InvalidOperationException("Category cannot be its own parent.");
                }

                if (!await _repository.ExistsAsync(dto.ParentCategoryId.Value))
                {
                    throw new InvalidOperationException($"Parent category with ID {dto.ParentCategoryId.Value} does not exist.");
                }
            }

            existing.Name = dto.Name;
            existing.Image = dto.Image;
            existing.ParentCategoryId = dto.ParentCategoryId;

            var updated = await _repository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (!await _repository.ExistsAsync(id))
            {
                throw new InvalidOperationException($"Category with ID {id} not found.");
            }

            return await _repository.DeleteAsync(id);
        }

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name
            };
        }

        private void ValidateCreateDto(CreateCategoryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Category name is required.", nameof(dto.Name));

            if (dto.Name.Length > 50)
                throw new ArgumentException("Category name cannot exceed 50 characters.", nameof(dto.Name));
        }

        private void ValidateUpdateDto(UpdateCategoryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Id <= 0)
                throw new ArgumentException("Invalid category ID.", nameof(dto.Id));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Category name is required.", nameof(dto.Name));

            if (dto.Name.Length > 50)
                throw new ArgumentException("Category name cannot exceed 50 characters.", nameof(dto.Name));
        }
    }
}

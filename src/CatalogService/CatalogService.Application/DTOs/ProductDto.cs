// <copyright file="ProductDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public CategoryDto Category { get; set; } = null!;
}

public class CreateProductDto
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int CategoryId { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int CategoryId { get; set; }
}

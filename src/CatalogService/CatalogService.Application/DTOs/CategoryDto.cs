// <copyright file="CategoryDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CatalogService.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}

public class CreateCategoryDto
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}

public class UpdateCategoryDto
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}

using CatalogService.API.Controllers;
using CatalogService.Application.DTOs;
using CatalogService.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;

namespace CatalogService.Tests.Integration;

public class HateoasControllerTests
{
    [Fact]
    public async Task ProductsController_GetById_Should_Return_Data_And_Links()
    {
        // Arrange
        var prod = new ProductDto { Id = 5, Name = "X", Description = "d", Price = 10m, CategoryId = 1 };
        var serviceMock = new Mock<IProductService>();
        serviceMock.Setup(s => s.GetProductByIdAsync(5)).ReturnsAsync(prod);

        var controller = new ProductsController(serviceMock.Object);

        // Provide a fake Url helper so Url.Action returns a fixed string
        var urlMock = new Mock<IUrlHelper>();
        urlMock.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/fake/url");
        controller.Url = urlMock.Object;

        // Act
        var result = await controller.Get(5);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.Should().NotBeNull();

        var valType = ok.Value!.GetType();
        var dataProp = valType.GetProperty("data");
        var linksProp = valType.GetProperty("links");

        dataProp.Should().NotBeNull();
        linksProp.Should().NotBeNull();

        var dataVal = dataProp.GetValue(ok.Value);
        dataVal.Should().NotBeNull();

        var linksVal = linksProp.GetValue(ok.Value) as System.Collections.IEnumerable;
        linksVal.Should().NotBeNull();

        // Ensure at least one link has href populated (our fake url)
        var enumerator = linksVal!.GetEnumerator();
        enumerator.MoveNext().Should().BeTrue();
        var first = enumerator.Current;
        var hrefProp = first!.GetType().GetProperty("href");
        hrefProp.Should().NotBeNull();
        var href = hrefProp!.GetValue(first) as string;
        href.Should().Be("/fake/url");
    }
}

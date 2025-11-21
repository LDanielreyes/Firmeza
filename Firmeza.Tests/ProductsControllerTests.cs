using AutoMapper;
using Firmeza.Data;
using Firmeza.Data.Entities;
using FirmezaAPI.Controllers;
using FirmezaAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Firmeza.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly ApplicationDbContext _context;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockMapper = new Mock<IMapper>();
            _controller = new ProductsController(_context, _mockMapper.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            _context.Products.Add(new Product { Id = 1, Name = "Product 1", Price = 10, Stock = 5, Type = "Type1" });
            _context.Products.Add(new Product { Id = 2, Name = "Product 2", Price = 20, Stock = 10, Type = "Type2" });
            await _context.SaveChangesAsync();

            var productDtos = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Product 1", Price = 10, Stock = 5, Type = "Type1" },
                new ProductDto { Id = 2, Name = "Product 2", Price = 20, Stock = 10, Type = "Type2" }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(productDtos);

            // Act
            var result = await _controller.GetProducts(null, null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count());
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Act
            var result = await _controller.GetProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostProduct_ReturnsCreatedAtAction_WhenProductIsCreated()
        {
            // Arrange
            var productDto = new ProductDto { Name = "New Product", Price = 30, Stock = 15, Type = "Type3" };
            var product = new Product { Id = 1, Name = "New Product", Price = 30, Stock = 15, Type = "Type3" };

            _mockMapper.Setup(m => m.Map<Product>(productDto)).Returns(product);
            _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _controller.PostProduct(productDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
            Assert.Equal(product.Id, createdAtActionResult.RouteValues!["id"]);
        }
    }
}

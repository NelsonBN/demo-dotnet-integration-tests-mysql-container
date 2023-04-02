using System.Net.Http.Json;
using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace Demo.Tests;

public class Tests
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public Tests(ITestOutputHelper output)
    {
        _client = new WebApplicationFactory<ProductRequest>().CreateClient();
        _output = output;
    }


    [Fact]
    public async Task TestConnection()
    {
        // Arrange && Act
        var act = await _client.GetAsync("/connection");
        var apiConnectionString = await act.Content.ReadAsAsync(typeof(string)) as string;


        // Assert
        _output.WriteLine($"Api ConnectionString: {apiConnectionString}");

        act.Should()
            .Be200Ok()
            .And.Satisfy<string>(model =>
                model.Should().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public async Task AllProducts_Get_StatusCode200AndMoreThanOrEqualThree()
    {
        // Arrange && Act
        var act = await _client.GetAsync("/products");


        // Assert
        act.Should()
           .Be200Ok();

        //act.Should()
        //   .Be200Ok()
        //   .And.Satisfy<IEnumerable<ProductResponse>>(model =>
        //        model.Should().HaveCountGreaterThanOrEqualTo(3));
    }

    [Fact]
    public async Task ProductId1_Get_StatusCode200AndProduct()
    {
        // Arrange
        var id = 2;


        // Act
        var act = await _client.GetAsync($"/products/{id}");


        // Assert
        act.Should()
           .Be200Ok();

        //act.Should()
        //   .Be200Ok()
        //   .And.Satisfy<ProductResponse>(model =>
        //        model.Should().Match<ProductResponse>(m =>
        //            m.Id == 2 &&
        //            m.Name == "Keyboard" &&
        //            m.Quantity == 4));
    }

    [Fact]
    public async Task NewPrduct_Post_StatusCode201AndId()
    {
        // Arrange
        var product = new Faker<ProductRequest>()
            .RuleFor(p => p.Name, s => s.Commerce.ProductName())
            .RuleFor(p => p.Quantity, s => s.Random.Int(1, 100))
            .Generate();


        // Act
        var act = await _client.PostAsync(
            "/products",
            JsonContent.Create(product));


        // Assert
        act.Should()
           .Be201Created()
           .And.Satisfy<ulong>(model =>
                model.Should().BeGreaterThan(0));
    }
}

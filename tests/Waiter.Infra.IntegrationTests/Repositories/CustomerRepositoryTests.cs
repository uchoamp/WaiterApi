using Microsoft.EntityFrameworkCore;
using Waiter.Application.Models.Customers;
using Waiter.Domain.Models;
using Waiter.Infra.Data;
using Waiter.Infra.Repositories;

namespace Waiter.Infra.IntegrationTests.Repositories;

public sealed class CustomerRepositoryTests : IDisposable
{
    private readonly CustomerRepository _sut;
    private readonly ApplicationDbContext _dbContext;

    public CustomerRepositoryTests()
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(this.GetHashCode().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(dbOptions);

        _sut = new CustomerRepository(_dbContext);

        PopulateDatabase();
    }

    [Theory]
    [InlineData(1, 1, 5, 1, 5, "5")]
    [InlineData(3, 2, 5, 1, 3, "1")]
    public async Task ShouldPaginateCorretly(
        int currentPage,
        int pageSize,
        int totalExpected,
        int pageResultExpected,
        int lastPageExpected,
        string expectedLastName
    )
    {
        var result = await _sut.PaginateAsync(new PaginationLimits(currentPage, pageSize));

        result.TotalEntities.Should().Be(totalExpected);
        result.Result.Should().HaveCount(pageResultExpected);
        result.PageSize.Should().Be(pageSize);
        result.CurrentPage.Should().Be(currentPage);
        result.LastPage.Should().Be(lastPageExpected);
        result.Result[0].LastName.Should().Be(expectedLastName);
    }

    [Theory]
    [InlineData("Foo", 1, 1, 1, 1, 1, "1")]
    [InlineData("Bar", 1, 2, 3, 2, 2, "4")]
    public async Task ShouldPaginateAndDFilterCorretly(
        string nameFilter,
        int currentPage,
        int pageSize,
        int totalExpected,
        int pageResultExpected,
        int lastPageExpected,
        string expectedLastName
    )
    {
        var result = await _sut.FilterAndPaginateAsync(
            new CustomerFilter(nameFilter),
            new PaginationLimits(currentPage, pageSize)
        );

        result.TotalEntities.Should().Be(totalExpected);
        result.Result.Should().HaveCount(pageResultExpected);
        result.PageSize.Should().Be(pageSize);
        result.CurrentPage.Should().Be(currentPage);
        result.LastPage.Should().Be(lastPageExpected);
        result.Result[0].LastName.Should().Be(expectedLastName);
    }

    private void PopulateDatabase()
    {
        _dbContext.Add(
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Teste Foo",
                LastName = "1",
                PhoneNumber = "",
                CreatedAt = DateTime.UtcNow
            }
        );

        _dbContext.Add(
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Teste Bar",
                LastName = "2",
                PhoneNumber = "",
                CreatedAt = DateTime.UtcNow
            }
        );

        _dbContext.Add(
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Teste Bar",
                LastName = "3",
                PhoneNumber = "",
                CreatedAt = DateTime.UtcNow
            }
        );

        _dbContext.Add(
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Teste Bar",
                LastName = "4",
                PhoneNumber = "",
                CreatedAt = DateTime.UtcNow
            }
        );

        _dbContext.Add(
            new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = "Teste",
                LastName = "5",
                PhoneNumber = "",
                CreatedAt = DateTime.UtcNow
            }
        );

        _dbContext.SaveChanges();
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}

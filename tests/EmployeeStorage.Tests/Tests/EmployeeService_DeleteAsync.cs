using EmployeeStorage.API.Domain.EmployeeAggregate;
using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Infrastructure.Services;
using EmployeeStorage.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace EmployeeStorage.Tests.Tests;

public class EmployeeService_DeleteAsync
{
    private readonly Mock<IMemoryCache> _mockCache;

    public EmployeeService_DeleteAsync()
    {
        _mockCache = new Mock<IMemoryCache>();
        _mockCache.Setup(cache => cache.Remove(It.IsAny<(int, int)>()));
    }

    [Fact]
    public async void DeleteAsync_Id_ResultSuccess()
    {
        // Arrange
        int id = 1;

        var transaction = new MockTransaction();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .Returns(Task.Run<EmployeeExtended?>(() => GetEmployeeExtended()));
        mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var result = await employeeService.DeleteAsync(id);

        // Assert
        Assert.IsType<Result<bool>>(result);
        Assert.False(result.IsFailure);
        Assert.True(result.Value);
    }

    [Fact]
    public async void DeleteAsync_Id_ResultFailure()
    {
        // Arrange
        int id = 1;

        var transaction = new MockTransaction();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var result = await employeeService.DeleteAsync(id);

        // Assert
        Assert.IsType<Result<bool>>(result);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async void DeleteAsync_Id_InvalidOperationException()
    {
        // Arrange
        int id = 1;

        var transaction = new MockTransactionException();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .Returns(Task.Run<EmployeeExtended?>(() => GetEmployeeExtended()));
        mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var act = () => employeeService.DeleteAsync(id);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    private EmployeeExtended GetEmployeeExtended() =>
        new EmployeeExtended
        {
            Id = 1,
            Name = "Иван",
            Surname = "Иванов",
            Phone = "+78005553555",
            CompanyId = 1,
            Passport = new Passport
            {
                Type = "ru",
                Number = "4018172065"
            },
            Department = new Department
            {
                Name = "Маркетинг",
                Phone = "+78005553555"
            },
            DepartmentId = 1
        };
}

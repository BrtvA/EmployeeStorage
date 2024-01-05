using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Domain.EmployeeAggregate;
using EmployeeStorage.API.Infrastructure.DataContracts;
using EmployeeStorage.API.Infrastructure.Services;
using EmployeeStorage.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace EmployeeStorage.Tests.Tests;

public class EmployeeService_CreateAsync
{
    private readonly Mock<IMemoryCache> _mockCache;

    private CreateRequest CreateRequest =>
        new CreateRequest
        {
            Name = "Иван",
            Surname = "Иванов",
            Phone = "+78005553555",
            CompanyId = 1,
            Passport = new Passport
            {
                Type = "ru",
                Number = "4018172065"
            },
            DepartmentId = 1
        };

    public EmployeeService_CreateAsync()
    {
        _mockCache = new Mock<IMemoryCache>();
        _mockCache.Setup(cache => cache.Remove(It.IsAny<(int, int)>()));
    }

    [Fact]
    public async void CreateAsync_CreateRequest_ResultSuccess()
    {
        // Arrange
        var transaction = new MockTransaction();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetByPassportAsync(It.IsAny<Passport>()));
        mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<EmployeeExtended>()))
            .Returns(Task.Run(() => 1));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var result = await employeeService.CreateAsync(CreateRequest);

        // Assert
        Assert.IsType<Result<int>>(result);
        Assert.False(result.IsFailure);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async void CreateAsync_CreateRequest_ResultFailure()
    {
        // Arrange
        var transaction = new MockTransaction();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetByPassportAsync(It.IsAny<Passport>()))
            .Returns(Task.Run<Employee?>(() => GetEmployee()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var result = await employeeService.CreateAsync(CreateRequest);

        // Assert
        Assert.IsType<Result<int>>(result);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async void CreateAsync_CreateRequest_InvalidOperationException()
    {
        // Arrange
        var transaction = new MockTransactionException();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetByPassportAsync(It.IsAny<Passport>()));
        mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<EmployeeExtended>()))
            .Returns(Task.Run(() => 1));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var act = () => employeeService.CreateAsync(CreateRequest);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    private Employee GetEmployee() =>
        new Employee
        {
            Id = 1,
            Name = "Иван",
            Surname = "Иванов",
            Phone = "+78005553555",
            CompanyId = 1,
            Passport = CreateRequest.Passport,
            Department = new Department
            {
                Name = "Маркетинг",
                Phone = "+78005553555"
            }
        };
}

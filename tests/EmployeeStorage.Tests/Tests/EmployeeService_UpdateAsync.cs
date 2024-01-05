﻿using EmployeeStorage.API.Domain.EmployeeAggregate;
using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Infrastructure.Services;
using Moq;
using EmployeeStorage.API.Infrastructure.DataContracts;
using EmployeeStorage.Tests.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeStorage.Tests.Tests;

public class EmployeeService_UpdateAsync
{
    private readonly Mock<IMemoryCache> _mockCache;

    private UpdateRequest UpdateRequest =>
        new UpdateRequest
        {
            Passport = new Passport
            {
                Type = "eu"
            }
        };

    public EmployeeService_UpdateAsync()
    {
        _mockCache = new Mock<IMemoryCache>();
        _mockCache.Setup(cache => cache.Remove(It.IsAny<(int, int)>()));
    }

    [Fact]
    public async void UpdateAsync_IdUpdateRequest_ResultSuccess()
    {
        // Arrange
        var mockRepository = new Mock<IEmployeeRepository>();
        var transaction = new MockTransaction();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .Returns(Task.Run<EmployeeExtended?>(() => GetEmployeeExtended()));
        mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<EmployeeExtended>()));
        mockRepository.Setup(repo => repo.GetByPassportAsync(It.IsAny<Passport>()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var resultService = await employeeService.UpdateAsync(1, UpdateRequest);

        // Assert
        Assert.IsType<Result<bool>>(resultService);
        Assert.False(resultService.IsFailure);
        Assert.True(resultService.Value);
    }

    [Fact]
    public async void UpdateAsync_IdUpdateRequest_ResultFailure404()
    {
        // Arrange
        var mockRepository = new Mock<IEmployeeRepository>();
        var transaction = new MockTransaction();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var resultService = await employeeService.UpdateAsync(1, UpdateRequest);

        // Assert
        Assert.IsType<Result<bool>>(resultService);
        Assert.True(resultService.IsFailure);
        Assert.Equal(404, resultService.StatusCode);
    }

    [Fact]
    public async void UpdateAsync_IdUpdateRequest_ResultFailure400()
    {
        // Arrange
        var mockRepository = new Mock<IEmployeeRepository>();
        var transaction = new MockTransaction();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .Returns(Task.Run<EmployeeExtended?>(() => GetEmployeeExtended()));
        mockRepository.Setup(repo => repo.GetByPassportAsync(It.IsAny<Passport>()))
            .Returns(Task.Run<Employee?>(() => GetEmployee()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var resultService = await employeeService.UpdateAsync(1, UpdateRequest);

        // Assert
        Assert.IsType<Result<bool>>(resultService);
        Assert.True(resultService.IsFailure);
        Assert.Equal(400, resultService.StatusCode);
    }

    [Fact]
    public async void UpdateAsync_IdUpdateRequest_InvalidOperationException()
    {
        // Arrange
        var mockRepository = new Mock<IEmployeeRepository>();
        var transaction = new MockTransactionException();
        mockRepository.Setup(repo => repo.BeginTransaction())
             .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .Returns(Task.Run<EmployeeExtended?>(() => GetEmployeeExtended()));
        mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<EmployeeExtended>()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var act = () => employeeService.UpdateAsync(1, UpdateRequest);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    private Employee GetEmployee() =>
        new Employee
        {
            Id = 2,
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
            }
        };

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


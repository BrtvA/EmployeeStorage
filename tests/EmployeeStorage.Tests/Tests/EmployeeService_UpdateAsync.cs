using EmployeeStorage.API.Domain.EmployeeAggregate;
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

    public static IEnumerable<object[]> UpdateRequestsList => new []
    {
        new object[] {
            new UpdateRequest
            {
                Name = "Михаил",
                Surname = "Смирнов",
                Phone = "+78005553111",
                CompanyId = 2,
                Passport = new Passport
                {
                    Type = "eu",
                    Number = "4018172065"
                },
                DepartmentId = 2
            }
        },
        [
            new UpdateRequest
            {
                Name = "Михаил",
            }
        ],
        [
            new UpdateRequest
            {
                Surname = "Смирнов"
            }
        ],
        [
            new UpdateRequest
            {
                Phone = "+78005553111"
            }
        ],
        [
            new UpdateRequest
            {
                CompanyId = 2,
            }
        ],
        [
            new UpdateRequest
            {
                Passport = new Passport
                {
                    Type = "eu"
                }
            } 
        ],
        [
            new UpdateRequest
            {
                Passport = new Passport
                {
                    Number = "4018172065"
                }
            } 
        ],
        [
            new UpdateRequest
            {
                DepartmentId = 2
            }
        ],
    };

    public EmployeeService_UpdateAsync()
    {
        _mockCache = new Mock<IMemoryCache>();
        _mockCache.Setup(cache => cache.Remove(It.IsAny<(int, int)>()));
    }

    [Theory]
    [MemberData(nameof(UpdateRequestsList))]
    public async void UpdateAsync_IdUpdateRequest_ResultSuccess(UpdateRequest updateRequest)
    {
        // Arrange
        int id = 1;

        var transaction = new MockTransaction();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .Returns(Task.Run<EmployeeExtended?>(() => GetEmployeeExtended()));
        mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<EmployeeExtended>()));
        mockRepository.Setup(repo => repo.GetByPassportAsync(It.IsAny<Passport>()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        //var resultService = await employeeService.UpdateAsync(id, UpdateRequest);
        var resultService = await employeeService.UpdateAsync(id, updateRequest);

        // Assert
        Assert.IsType<Result<bool>>(resultService);
        Assert.False(resultService.IsFailure);
        Assert.True(resultService.Value);
    }

    [Fact]
    public async void UpdateAsync_IdUpdateRequest_ResultFailure404()
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
        var resultService = await employeeService.UpdateAsync(id, UpdateRequest);

        // Assert
        Assert.IsType<Result<bool>>(resultService);
        Assert.True(resultService.IsFailure);
        Assert.Equal(404, resultService.StatusCode);
    }

    [Fact]
    public async void UpdateAsync_IdUpdateRequest_ResultFailure400()
    {
        // Arrange
        int id = 1;

        var transaction = new MockTransaction();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
            .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .Returns(Task.Run<EmployeeExtended?>(() => GetEmployeeExtended()));
        mockRepository.Setup(repo => repo.GetByPassportAsync(It.IsAny<Passport>()))
            .Returns(Task.Run<Employee?>(() => GetEmployee()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var resultService = await employeeService.UpdateAsync(id, UpdateRequest);

        // Assert
        Assert.IsType<Result<bool>>(resultService);
        Assert.True(resultService.IsFailure);
        Assert.Equal(400, resultService.StatusCode);
    }

    [Fact]
    public async void UpdateAsync_IdUpdateRequest_InvalidOperationException()
    {
        // Arrange
        int id = 1;

        var transaction = new MockTransactionException();

        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.BeginTransaction())
             .Returns(transaction);
        mockRepository.Setup(repo => repo.GetAsync(It.IsAny<int>()))
            .Returns(Task.Run<EmployeeExtended?>(() => GetEmployeeExtended()));
        mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<EmployeeExtended>()));

        var employeeService = new EmployeeService(mockRepository.Object, _mockCache.Object);

        // Act
        var act = () => employeeService.UpdateAsync(id, UpdateRequest);

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


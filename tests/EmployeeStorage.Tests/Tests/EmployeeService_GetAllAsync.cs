using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Domain.EmployeeAggregate;
using EmployeeStorage.API.Infrastructure.Services;
using EmployeeStorage.Tests.Helpers;
using Moq;

namespace EmployeeStorage.Tests.Tests;

public class EmployeeService_GetAllAsync
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, null)]
    public async void GetAllAsync_CompanyIdDepartmentId_ResultCasheSuccess(
        int companyId, int? departmentId)
    {
        // Arrange
        var resultCache = Result<IEnumerable<Employee>>.Success(GetEmployees());

        var mockRepository = new Mock<IEmployeeRepository>();

        var cache = new MockMemoryCache(resultCache);


        var employeeService = new EmployeeService(mockRepository.Object, cache);

        // Act
        var resultService = await employeeService.GetAllAsync(companyId, departmentId);

        // Assert
        Assert.IsType<Result<IEnumerable<Employee>>>(resultService);
        Assert.False(resultService.IsFailure);
        Assert.Equal(resultCache.Value?.Count(), resultService.Value?.Count());
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, null)]
    public async void GetAllAsync_CompanyIdDepartmentId_ResultSuccess(
        int companyId, int? departmentId)
    {
        // Arrange
        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int?>()))
            .Returns(Task.Run(() => GetEmployees()));

        var cache = new MockMemoryCache(null);

        var employeeService = new EmployeeService(mockRepository.Object, cache);

        // Act
        var resultService = await employeeService.GetAllAsync(companyId, departmentId);

        // Assert
        Assert.IsType<Result<IEnumerable<Employee>>>(resultService);
        Assert.False(resultService.IsFailure);
        Assert.Equal(GetEmployees().Count(), resultService.Value?.Count());
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, null)]
    public async void GetAllAsync_CompanyIdDepartmentId_ResultFailure(
        int companyId, int? departmentId)
    {
        // Arrange
        var mockRepository = new Mock<IEmployeeRepository>();
        mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int?>()))
            .Returns(Task.Run(() => GetEmptyEmployees()));

        var cache = new MockMemoryCache(null);

        var employeeService = new EmployeeService(mockRepository.Object, cache);

        // Act
        var resultService = await employeeService.GetAllAsync(companyId, departmentId);

        // Assert
        Assert.IsType<Result<IEnumerable<Employee>>>(resultService);
        Assert.True(resultService.IsFailure);
    }

    private IEnumerable<Employee> GetEmptyEmployees()
    {
        return new List<Employee>();
    }

    private IEnumerable<Employee> GetEmployees()
    {
        return new List<Employee>
        {
            new Employee
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
                }
            },
            new Employee
            {
                Id = 2,
                Name = "Петр",
                Surname = "Петров",
                Phone = "+78005553555",
                CompanyId = 1,
                Passport = new Passport
                {
                    Type = "ru",
                    Number = "4018172055"
                },
                Department = new Department
                {
                    Name = "Маркетинг",
                    Phone = "+78005553555"
                }
            }
        };
    }
}

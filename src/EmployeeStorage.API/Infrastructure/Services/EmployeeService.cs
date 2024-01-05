using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Domain.EmployeeAggregate;
using EmployeeStorage.API.Infrastructure.DataContracts;
using EmployeeStorage.API.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeStorage.API.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMemoryCache _cache;

    public EmployeeService(IEmployeeRepository employeeRepository, IMemoryCache cache)
    {
        _employeeRepository = employeeRepository;
        _cache = cache;
    }

    public async Task<Result<int>> CreateAsync(CreateRequest createRequest)
    {
        _employeeRepository.OpenConnection();

        using var transaction = _employeeRepository.BeginTransaction();
        try
        {
            Result<int> result;

            var employee = await _employeeRepository.GetByPassportAsync(createRequest.Passport);

            if (employee is not null)
            {
                result = Result<int>.Failure("Уже существует сотрудник с такими данными", 400);
            }
            else
            {
                int id = await _employeeRepository.CreateAsync(
                    createRequest.ConvertToEmployeeExtended()
                );

                result = Result<int>.Success(id);

                DeleteCache(createRequest.CompanyId, createRequest.DepartmentId);
            }

            transaction.Commit();

            return result;
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    public async Task<Result<IEnumerable<Employee>>> GetAllAsync(
        int companyId, int? departmentId)
    {
        _cache.TryGetValue((companyId, departmentId), out Result<IEnumerable<Employee>>? result);

        if (result is null)
        {
            var employees = await _employeeRepository.GetAllAsync(
                companyId, departmentId
            );

            if (!employees.Any())
            {
                return Result<IEnumerable<Employee>>.Failure("Нет данных", 404);
            }

            result = Result<IEnumerable<Employee>>.Success(employees);

            _cache.Set((companyId, departmentId), result, 
                new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
            );
        }

        return result;
    }

    public async Task<Result<bool>> UpdateAsync(int id, UpdateRequest updateRequest)
    {
        _employeeRepository.OpenConnection();

        using var transaction = _employeeRepository.BeginTransaction();
        try
        {
            Result<bool> result;

            var employee = await _employeeRepository.GetAsync(id);

            if (employee is null)
            {
                result = Result<bool>.Failure("Нет такого сотрудника", 404);
            }
            else
            {
                var data = updateRequest.MergeWithCurrentData(id, employee);

                var employee2 = await _employeeRepository.GetByPassportAsync(data.Passport);
                
                if (employee2 is null || employee.Id == employee2.Id)
                {
                    await _employeeRepository.UpdateAsync(data);

                    result = Result<bool>.Success(true);

                    DeleteCache(employee.CompanyId, employee.DepartmentId);
                }
                else
                {
                    result = Result<bool>.Failure("Уже существует сотрудник с такими данными", 400);
                }
            }

            transaction.Commit();

            return result;
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        _employeeRepository.OpenConnection();
        using var transaction = _employeeRepository.BeginTransaction();
        try
        {
            Result<bool> result;

            var employee = await _employeeRepository.GetAsync(id);

            if (employee is null)
            {
                result = Result<bool>.Failure("Нет такого сотрудника", 404);
            }
            else
            {
                await _employeeRepository.DeleteAsync(id);

                result = Result<bool>.Success(true);

                DeleteCache(employee.CompanyId, employee.DepartmentId);
            }

            transaction.Commit();

            return result;
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    private void DeleteCache(int companyId, int departmentId)
    {
        _cache.Remove((companyId, departmentId));
        int? key = null;
        _cache.Remove((companyId, key));
    }
}

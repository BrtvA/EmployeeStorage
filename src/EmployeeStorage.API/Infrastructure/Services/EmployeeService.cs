using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Infrastructure.DataContracts;
using EmployeeStorage.API.Infrastructure.Services.Interfaces;

namespace EmployeeStorage.API.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
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
                    createRequest.ConvertToEmployee(), createRequest.DepartmentId
                );

                result = Result<int>.Success(id);
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
        var result = await _employeeRepository.GetAllAsync(
            companyId, departmentId
        );

        if (!result.Any())
        {
            Result<IEnumerable<Employee>>.Failure("Нет данных", 404);
        }

        return Result<IEnumerable<Employee>>.Success(result);
    }

    public async Task<Result<bool>> UpdateAsync(int id, UpdateRequest updateRequest)
    {
        _employeeRepository.OpenConnection();

        using var transaction = _employeeRepository.BeginTransaction();
        try
        {
            Result<bool> result;

            var employee = await _employeeRepository.Get(id);

            if (employee is null)
            {
                result = Result<bool>.Failure("Нет такого сотрудника", 404);
            }
            else
            {
                var data = new Employee() 
                {
                    Id = id,
                    Name = updateRequest.Name ?? employee.Name,
                    Surname = updateRequest.Surname ?? employee.Surname,
                    Phone = updateRequest.Phone ?? employee.Phone,
                    CompanyId = updateRequest.CompanyId ?? employee.CompanyId,
                    Passport = updateRequest.Passport is not null 
                        ? new Passport
                        {
                            Type = updateRequest.Passport.Type ?? employee.PassportType,
                            Number = updateRequest.Passport.Number ?? employee.PassportNumber
                        }
                        : new Passport
                        {
                            Type = employee.PassportType,
                            Number = employee.PassportNumber
                        }        
                };


                await _employeeRepository.UpdateAsync(
                    data, 
                    updateRequest.DepartmentId ?? employee.DepartmentId
                );

                result = Result<bool>.Success(true);
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

            var employee = await _employeeRepository.Get(id);

            if (employee is null)
            {
                result = Result<bool>.Failure("Нет такого сотрудника", 404);
            }
            else
            {
                await _employeeRepository.DeleteAsync(id);

                result = Result<bool>.Success(true);
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
}

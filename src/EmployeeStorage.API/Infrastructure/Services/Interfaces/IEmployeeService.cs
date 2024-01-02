using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Infrastructure.DataContracts;

namespace EmployeeStorage.API.Infrastructure.Services.Interfaces;

public interface IEmployeeService
{
    public Task<Result<int>> CreateAsync(CreateRequest createRequest);
    public Task<Result<IEnumerable<Employee>>> GetAllAsync(
        int companyId, int? departmentId
    );
    public Task<Result<bool>> UpdateAsync(int id, UpdateRequest updateRequest);
    public Task<Result<bool>> DeleteAsync(int id);
}

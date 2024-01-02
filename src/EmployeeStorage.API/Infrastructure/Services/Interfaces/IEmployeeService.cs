using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Infrastructure.DataContracts;

namespace EmployeeStorage.API.Infrastructure.Services.Interfaces;

public interface IEmployeeService
{
    public Task<Result<int>> CreateAsync(CreateRequest createRequest);
    public Task<Result<IEnumerable<Employee>>> GetAllByCompanyAsync(GetRequest getRequest);
    public Task<Result<IEnumerable<Employee>>> GetAllByDepartmentAsync(GetFullRequest getRequest);
    public Task<Result<bool>> UpdateAsync(UpdateRequest updateRequest);
    public Task<Result<bool>> DeleteAsync(BaseRequest deleteRequest);
}

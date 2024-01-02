using System.Data;

namespace EmployeeStorage.API.Domain.EmployeeAggregate;

public interface IEmployeeRepository : IDisposable
{
    public void OpenConnection();
    public IDbTransaction BeginTransaction();
    public Task<int> CreateAsync(Employee employee, int departmentId);
    public Task<EmployeeExtended?> Get(int id);
    public Task<IEnumerable<Employee>> GetAllAsync(
        int companyId, int? departmentId);
    public Task<Employee?> GetByPassportAsync(Passport passport);
    public Task UpdateAsync(Employee employee, int departmentId);
    public Task DeleteAsync(int id);
}
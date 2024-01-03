using EmployeeStorage.API.Domain.EmployeeAggregate;
using System.Data;

namespace EmployeeStorage.API.Domain;

public interface IEmployeeRepository : IDisposable
{
    public void OpenConnection();
    public IDbTransaction BeginTransaction();
    public Task<int> CreateAsync(Employee employee, int departmentId);
    public Task<EmployeeExtended?> Get(int id);
    public Task<IEnumerable<Employee>> GetAllAsync(
        int companyId, int? departmentId);
    public Task<Employee?> GetByPassportAsync(Passport passport);
    public Task UpdateAsync(EmployeeExtended employeeExtended);
    public Task DeleteAsync(int id);
}
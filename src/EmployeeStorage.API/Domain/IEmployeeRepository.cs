using System.Data;

namespace EmployeeStorage.API.Domain;

public interface IEmployeeRepository : IDisposable
{
    public void OpenConnection();
    public IDbTransaction BeginTransaction();
    public Task<int> CreateAsync(Employee employee, int departmentId);
    public Task<EmployeeExtended?> Get(int id);
    public Task<IEnumerable<Employee>> GetAllByCompanyAsync(string companyName);
    public Task<IEnumerable<Employee>> GetAllByDepartmentAsync(
        string companyName, string departmentName);
    public Task<Employee?> GetByPassportAsync(Passport passport);
    public Task UpdateAsync(Employee employee, int departmentId);
    public Task DeleteAsync(int id);
}
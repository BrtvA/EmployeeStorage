using EmployeeStorage.API.Domain.EmployeeAggregate;

namespace EmployeeStorage.API.Domain
{
    public class EmployeeExtended : Employee
    {
        public int DepartmentId { get; set; }
    }
}

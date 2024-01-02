namespace EmployeeStorage.API.Domain
{
    public class EmployeeExtended
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int CompanyId { get; set; }
        public string PassportType { get; set; } = null!;
        public string PassportNumber { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string DepartmentPhone { get; set; } = null!;
    }
}

using Dapper;
using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Domain.EmployeeAggregate;
using EmployeeStorage.API.Infrastructure.DataBase;
using System.Data;

namespace EmployeeStorage.API.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private DataContext _context;
    private IDbConnection _connection;

    public EmployeeRepository(DataContext context)
    {
        _context = context;
        _connection = _context.CreateConnection();
    }

    public void OpenConnection()
    {
        _connection.Open();
    }

    public IDbTransaction BeginTransaction()
    {
        return _connection.BeginTransaction();
    }

    public async Task<int> CreateAsync(Employee employee, int departmentId)
    {
        return await _connection.QuerySingleAsync<int>(
            """
            INSERT INTO "Employees" ("Name", "Surname", "Phone", 
                                     "CompanyId", "PassportType", 
                                     "PassportNumber", "DepartmentId")
            VALUES (@Name, @Surname, @Phone, 
                    @CompanyId, @PassportType, 
                    @PassportNumber, @DepartmentId)
            RETURNING "Id"; 
            """,
            new
            {
                employee.Name,
                employee.Surname,
                employee.Phone,
                employee.CompanyId,
                PassportType = employee.Passport.Type,
                PassportNumber = employee.Passport.Number,
                DepartmentId = departmentId
            }
        );
    }

    public async Task<EmployeeExtended?> Get(int id)
    {
        var result = await _connection.QueryAsync<EmployeeExtended, Passport, Department, EmployeeExtended>(
            """
            SELECT e."Id", e."Name", e."Surname", 
              	   e."Phone", e."CompanyId",
                   d."Id" as "DepartmentId",
                   e."PassportType" as "Type", e."PassportNumber" as "Number",
                   d."Name", d."Phone"
            FROM "Employees" as e
            JOIN "Departments" as d
                ON e."DepartmentId" = d."Id"
            WHERE e."Id" = @id
            """,
            map: (employeeExtended, passport, department) =>
            {
                employeeExtended.Passport = passport;
                employeeExtended.Department = department;
                return employeeExtended;
            },
            param: new { id },
            splitOn: "Type, Name"
        );

        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Employee>> GetAllAsync(
        int companyId, int? departmentId)
    {
        var results = await _connection.QueryAsync<Employee, Passport, Department, Employee>(
            $"""
            SELECT e."Id", e."Name", e."Surname", 
              	   e."Phone", e."CompanyId", 
                   e."PassportType" as "Type", e."PassportNumber" as "Number",
                   d."Name", d."Phone"
            FROM "Employees" as e
            JOIN "Departments" as d
                ON e."DepartmentId" = d."Id"
            WHERE e."CompanyId" = @companyId
            {(departmentId is not null ? "AND e.\"DepartmentId\" = @departmentId" : "")};
            """,
            map: (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            },
            param: new
            {
                companyId,
                departmentId
            },
            splitOn: "Type, Name"
        );
        return results;
    }

    public async Task<Employee?> GetByPassportAsync(Passport passport)
    {
        return await _connection.QuerySingleOrDefaultAsync<Employee>(
            """
            SELECT * FROM "Employees"
            WHERE "PassportType" = @Type
            AND "PassportNumber" = @Number
            """, passport
        );
    }

    public async Task UpdateAsync(EmployeeExtended employeeExtended)
    {
        await _connection.ExecuteAsync(
            """
            UPDATE "Employees" 
            SET "Name" = @Name,
                "Surname" = @Surname,
                "Phone" = @Phone,
                "CompanyId" = @CompanyId,
                "PassportType" = @PassportType,
                "PassportNumber" = @PassportNumber,
                "DepartmentId" = @DepartmentId
            WHERE "Id" = @Id
            """,
            new
            {
                employeeExtended.Id,
                employeeExtended.Name,
                employeeExtended.Surname,
                employeeExtended.Phone,
                employeeExtended.CompanyId,
                PassportType = employeeExtended.Passport.Type,
                PassportNumber = employeeExtended.Passport.Number,
                employeeExtended.DepartmentId
            }
        );
    }

    public async Task DeleteAsync(int id)
    {
        await _connection.ExecuteAsync(
            """
            DELETE FROM "Employees" 
            WHERE "Id" = @id
            """, new { id }
        );
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool disposed = false;
    public virtual void Dispose(bool disposing)
    {

        if (disposed) return;
        if (disposing)
        {
            _connection.Dispose();
        }
        disposed = true;
    }

    ~EmployeeRepository()
    {
        Dispose(false);
    }
}

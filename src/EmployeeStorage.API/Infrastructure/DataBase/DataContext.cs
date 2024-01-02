using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace EmployeeStorage.API.Infrastructure.DataBase;

public class DataContext
{
    private DbSettings _dbSettings;
    private ILogger<DataContext> _logger;

    public DataContext(IOptions<DbSettings> dbSettings,
        ILogger<DataContext> logger)
    {
        _dbSettings = dbSettings.Value;
        _logger = logger;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = $"""
            Host={_dbSettings.Server}; 
            Port={_dbSettings.Port}; 
            Database={_dbSettings.Database}; 
            Username={_dbSettings.UserId}; 
            Password={_dbSettings.Password};
            """;
        return new NpgsqlConnection(connectionString);
    }

    public async Task Init()
    {
        await InitDatabase();
        await InitTables();
        await InitData();
    }

    private async Task InitDatabase()
    {
        var connectionString = $"""
            Host={_dbSettings.Server}; 
            Port={_dbSettings.Port}; 
            Database=postgres; 
            Username={_dbSettings.UserId}; 
            Password={_dbSettings.Password};
            """;
        using var connection = new NpgsqlConnection(connectionString);

        var sqlDbCount = $"""
            SELECT COUNT(*) 
            FROM pg_database 
            WHERE datname = '{_dbSettings.Database}';
            """;
        var dbCount = await connection.ExecuteScalarAsync<int>(sqlDbCount);
        if (dbCount == 0)
        {
            var sql = $"CREATE DATABASE \"{_dbSettings.Database}\"";
            await connection.ExecuteAsync(sql);

            _logger.LogInformation("{DateTime}: База данных {Database} создана",
                DateTime.Now, _dbSettings.Database
            );
        }
    }

    private async Task InitTables()
    {
        using var connection = CreateConnection();

        await InitCompany();
        await InitDepartments();
        await InitEmployees();

        async Task InitCompany()
        {
            var status = await connection.QuerySingleAsync<bool>(
                """
                SELECT EXISTS(
                    SELECT * FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME='Companies'
                );
                """
            );

            if (!status)
            {
                var sql = """
                CREATE TABLE IF NOT EXISTS "Companies" (
                    "Id" SERIAL PRIMARY KEY,
                    "Name" VARCHAR(30) NOT NULL
                );
                """;
                await connection.ExecuteAsync(sql);

                _logger.LogInformation("{DateTime}: Таблица \"Companies\" создана",
                    DateTime.Now
                );
            }
        }

        async Task InitDepartments()
        {
            var status = await connection.QuerySingleAsync<bool>(
                """
                SELECT EXISTS(
                    SELECT * FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME='Departments'
                );
                """
            );

            if (!status)
            {
                var sql = """
                CREATE TABLE IF NOT EXISTS "Departments" (
                    "Id" SERIAL PRIMARY KEY,
                    "CompanyId" INTEGER NOT NULL,
                    "Name" VARCHAR(30) NOT NULL,
                    "Phone" CHAR(12) NOT NULL,
                    FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id")
                );
                """;
                await connection.ExecuteAsync(sql);

                _logger.LogInformation("{DateTime}: Таблица \"Departments\" создана",
                    DateTime.Now
                );
            }
        }

        async Task InitEmployees()
        {
            var status = await connection.QuerySingleAsync<bool>(
                """
                SELECT EXISTS(
                    SELECT * FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME='Employees'
                );
                """
            );

            if (!status)
            {
                var sql = """
                CREATE TABLE IF NOT EXISTS "Employees" (
                    "Id" SERIAL PRIMARY KEY,
                    "Name" VARCHAR(20) NOT NULL,
                    "Surname" VARCHAR(20) NOT NULL,
                    "Phone" VARCHAR(12) NOT NULL,
                    "CompanyId" INTEGER NOT NULL,
                    "PassportType" CHAR(2) NOT NULL,
                    "PassportNumber" CHAR(10) NOT NULL,
                    "DepartmentId" INTEGER NOT NULL,
                    FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id")
                );
                """;
                await connection.ExecuteAsync(sql);

                _logger.LogInformation("{DateTime}: Таблица \"Employees\" создана",
                   DateTime.Now
                );
            }
        }
    }

    private async Task InitData()
    {
        using var connection = CreateConnection();

        if (await InitDataCompanies()
            && await InitDataDepartments()
            && await InitDataEmployees())
        {
            _logger.LogInformation("{DateTime}: Начальные данные загружены",
                DateTime.Now
            );
        }

        async Task<bool> InitDataCompanies()
        {
            var dbCount = await connection.QuerySingleAsync<int>(
                """
                SELECT COUNT(*) FROM "Companies"
                """
            );

            if (dbCount == 0)
            {
                var sql = """
                    INSERT INTO "Companies" ("Name")
                    VALUES ('Рога и Копыта');
                    """;
                await connection.ExecuteAsync(sql);

                return true;
            }

            return false;
        }

        async Task<bool> InitDataDepartments()
        {
            var dbCount = await connection.QuerySingleAsync<int>(
                """
                SELECT COUNT(*) FROM "Departments"
                """
            );

            if (dbCount == 0)
            {
                var sql = """
                    INSERT INTO "Departments" ("Name", "CompanyId", "Phone")
                    VALUES ('Маркетинг', 1, '+78005553555');
                    """;
                await connection.ExecuteAsync(sql);

                return true;
            }

            return false;
        }

        async Task<bool> InitDataEmployees()
        {
            var dbCount = await connection.QuerySingleAsync<int>(
                """
                SELECT COUNT(*) FROM "Employees"
                """
            );

            if (dbCount == 0)
            {
                var sql = """
                    INSERT INTO "Employees" ("Name", "Surname", "Phone", 
                                             "CompanyId", "PassportType", 
                                             "PassportNumber", "DepartmentId")
                    VALUES ('Иван', 'Иванов', '+79885553545',
                            1, 'ru', '4018172065', 1);
                    """;
                await connection.ExecuteAsync(sql);

                return true;
            }

            return false;
        }
    }
}

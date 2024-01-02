using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Infrastructure.DataContracts;
using EmployeeStorage.API.Infrastructure.Services;
using EmployeeStorage.API.Infrastructure.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeStorage.API;

public static class EndPoints
{
    public static void DoEndPoint(this WebApplication app)
    {
        app.MapGet("/employee/company",

            async (IValidator<GetRequest> validator,
            IEmployeeService employeeService,
            [FromBody] GetRequest getRequest) =>
        {
            var validationResult = await validator.ValidateAsync(getRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await employeeService.GetAllByCompanyAsync(getRequest);

            return Results.Json(result);
        })
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Возвращает список сотрудников",
            Description = "Список сотрудников в определенной компании"
        })
        .Produces<Result<IEnumerable<Employee>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Accepts<GetRequest>("application/json");


        app.MapGet("/employee/department", async (IValidator<GetFullRequest> validator,
            IEmployeeService employeeService,
            [FromBody] GetFullRequest getRequest) =>
        {
            var validationResult = await validator.ValidateAsync(getRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await employeeService.GetAllByDepartmentAsync(getRequest);

            return Results.Json(result);
        })
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Возвращает список сотрудников",
            Description = "Список сотрудников в определенном отделе определенной компании"
        })
        .Produces<Result<IEnumerable<Employee>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Accepts<GetFullRequest>("application/json");


        app.MapPost("/employee", async (IValidator<CreateRequest> validator,
            IEmployeeService employeeService,
            [FromBody] CreateRequest createRequest) =>
        {
            var validationResult = await validator.ValidateAsync(createRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await employeeService.CreateAsync(createRequest);

            return Results.Json(result);
        })
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Создание сотрудника",
            Description = "Отправка информации для создания сотрудника"
        })
        .Produces<Result<int>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Accepts<CreateRequest>("application/json");


        app.MapPatch("/employee/{id}", async (IValidator<UpdateRequest> validator, 
            IEmployeeService employeeService,
            [FromBody] UpdateRequest updateRequest,
            int id) =>
        {
            if (id <= 0) return Results.BadRequest();

            var validationResult = await validator.ValidateAsync(updateRequest);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var result = await employeeService.UpdateAsync(id, updateRequest);

            return Results.Json(result);
        })
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Редактирование сотрудника",
            Description = "Отправка данных для редактирования информации о сотруднике"
        })
        .Produces<Result<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Accepts<UpdateRequest>("application/json");


        app.MapDelete("/employee/{id}", async (IEmployeeService employeeService,
            int id) =>
        {
            if (id <= 0) return Results.BadRequest();

            var result = await employeeService.DeleteAsync(id);

            return Results.Json(result);
        })
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Удаление сотрудника",
            Description = "Удаление сотрудника по его id"
        })
        .Produces<Result<bool>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}

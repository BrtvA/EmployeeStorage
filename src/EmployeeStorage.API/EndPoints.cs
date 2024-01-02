using EmployeeStorage.API.Domain.EmployeeAggregate;
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
        app.MapGet("/employee/company/{id}",
            async (IEmployeeService employeeService,
            int id) =>
        {
            if (id <= 0) return Results.BadRequest();

            var result = await employeeService.GetAllAsync(id, null);

            return Results.Json(result);
        })
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Возвращает список сотрудников",
            Description = "Список сотрудников в определенной компании"
        })
        .Produces<Result<IEnumerable<Employee>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        app.MapGet("/employee/company/{companyId}/department/{departmentId}",
            async (IEmployeeService employeeService,
            int companyId, int departmentId) =>
        {
            if (companyId <= 0 || departmentId <= 0) return Results.BadRequest();

            var result = await employeeService.GetAllAsync(companyId, departmentId);

            return Results.Json(result);
        })
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Возвращает список сотрудников",
            Description = "Список сотрудников в определенном отделе определенной компании"
        })
        .Produces<Result<IEnumerable<Employee>>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


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

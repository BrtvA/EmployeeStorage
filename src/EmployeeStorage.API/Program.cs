using EmployeeStorage.API;
using EmployeeStorage.API.Domain;
using EmployeeStorage.API.Infrastructure.Services.Interfaces;
using EmployeeStorage.API.Infrastructure.Services;
using EmployeeStorage.API.Infrastructure.Repositories;
using EmployeeStorage.API.Infrastructure.DataBase;
using FluentValidation;
using EmployeeStorage.API.Infrastructure.DataContracts;
using EmployeeStorage.API.Infrastructure.Validators;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using EmployeeStorage.API.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Emoloyee Storage API",
        Description = "ASP.NET Web API для учета сотрудников"
    });
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("DbSettings")
);

builder.Services.AddTransient<IValidator<CreateRequest>, CreateValidator>();
builder.Services.AddTransient<IValidator<UpdateRequest>, UpdateValidator>();

builder.Services.AddSingleton<DataContext>();
builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await context.Init();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.DoEndPoint();

app.Run();

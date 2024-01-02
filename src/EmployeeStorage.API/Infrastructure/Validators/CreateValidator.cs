using EmployeeStorage.API.Infrastructure.DataContracts;
using FluentValidation;

namespace EmployeeStorage.API.Infrastructure.Validators;

public class CreateValidator : AbstractValidator<CreateRequest>
{
    public CreateValidator()
    {
        RuleFor(createRequest => createRequest.Name)
            .NotNull()
            .NotEmpty()
            .Length(1, 20)
            .Matches(@"^[А-Яа-яЁё]+$");
        RuleFor(createRequest => createRequest.Surname)
            .NotNull()
            .NotEmpty()
            .Length(1, 20)
            .Matches(@"^[А-Яа-яЁё]+$");
        RuleFor(createRequest => createRequest.Phone)
            .NotNull()
            .NotEmpty()
            .Length(11, 12)
            .Matches(@"^(\+7|8)[0-9]{10}$");
        RuleFor(createRequest => createRequest.CompanyId)
            .NotNull()
            .GreaterThan(0);
        RuleFor(createRequest => createRequest.Passport)
            .NotNull();
        RuleFor(createRequest => createRequest.Passport.Type)
            .NotNull()
            .NotEmpty()
            .Length(2)
            .Matches(@"^[a-z]{2}$");
        RuleFor(createRequest => createRequest.Passport.Number)
            .NotNull()
            .NotEmpty()
            .Length(10)
            .Matches(@"^[0-9]{10}$");
        RuleFor(createRequest => createRequest.DepartmentId)
            .NotNull()
            .GreaterThan(0);
    }
}

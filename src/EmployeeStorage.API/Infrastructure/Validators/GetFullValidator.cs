using EmployeeStorage.API.Infrastructure.DataContracts;
using FluentValidation;

namespace EmployeeStorage.API.Infrastructure.Validators;

public class GetFullValidator : AbstractValidator<GetFullRequest>
{
    public GetFullValidator()
    {
        RuleFor(getRequest => getRequest.CompanyName)
            .NotNull()
            .NotEmpty()
            .Length(1, 30)
            .Matches(@"^[А-Яа-яЁёa-zA-Z ]+$");
        RuleFor(getRequest => getRequest.DepartmentName)
            .NotNull()
            .NotEmpty()
            .Length(1, 30)
            .Matches(@"^[А-Яа-яЁёa-zA-Z ]+$");
    }
}

using EmployeeStorage.API.Infrastructure.DataContracts;
using FluentValidation;

namespace EmployeeStorage.API.Infrastructure.Validators;

public class GetValidator : AbstractValidator<GetRequest>
{
    public GetValidator()
    {
        RuleFor(getRequest => getRequest.CompanyName)
            .NotNull()
            .NotEmpty()
            .Length(1, 30)
            .Matches(@"^[А-Яа-яЁёa-zA-Z ]+$");
    }
}

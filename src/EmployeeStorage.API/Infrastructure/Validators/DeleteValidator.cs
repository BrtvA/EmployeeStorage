using EmployeeStorage.API.Infrastructure.DataContracts;
using FluentValidation;

namespace EmployeeStorage.API.Infrastructure.Validators;

public class DeleteValidator : AbstractValidator<BaseRequest>
{
    public DeleteValidator()
    {
        RuleFor(x => x.Id).NotNull().GreaterThan(0);
    }
}

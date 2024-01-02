using EmployeeStorage.API.Infrastructure.DataContracts;
using FluentValidation;

namespace EmployeeStorage.API.Infrastructure.Validators;

public class UpdateValidator : AbstractValidator<UpdateRequest>
{
    public UpdateValidator()
    {
        RuleFor(updateRequest => updateRequest.Name)
            .NotEmpty()
            .Length(1, 20)
            .Matches(@"^[А-Яа-яЁё]+$")
            .When(updateRequest => updateRequest.Name is not null);
        RuleFor(updateRequest => updateRequest.Surname)
            .NotEmpty()
            .Length(1, 20)
            .Matches(@"^[А-Яа-яЁё]+$")
            .When(updateRequest => updateRequest.Surname is not null);
        RuleFor(updateRequest => updateRequest.Phone)
            .NotEmpty()
            .Length(11, 12)
            .Matches(@"^(\+7|8)[0-9]{10}$")
            .When(updateRequest => updateRequest.Phone is not null);
        RuleFor(updateRequest => updateRequest.CompanyId)
            .GreaterThan(0)
            .When(updateRequest => updateRequest.CompanyId is not null);
        RuleFor(updateRequest => updateRequest.Passport.Type)
            .NotEmpty()
            .Length(2)
            .Matches(@"^[a-z]{2}$")
            .When(updateRequest => updateRequest.Passport is not null 
                                && updateRequest.Passport.Type is not null);
        RuleFor(updateRequest => updateRequest.Passport.Number)
            .NotEmpty()
            .Length(10)
            .Matches(@"^[0-9]{10}$")
            .When(updateRequest => updateRequest.Passport is not null
                                && updateRequest.Passport.Number is not null);
    }
}

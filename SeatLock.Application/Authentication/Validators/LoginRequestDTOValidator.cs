using FluentValidation;
using SeatLock.Application.Authentication.DTO;

namespace SeatLock.Application.Authentication.Validators;

public sealed class LoginRequestDTOValidator : AbstractValidator<LoginRequestDTO>
{
    public LoginRequestDTOValidator()
    {
        RuleFor(value => value.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(value => value.Password).NotEmpty();
    }
}

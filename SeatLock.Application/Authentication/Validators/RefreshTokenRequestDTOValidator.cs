using FluentValidation;
using SeatLock.Application.Authentication.DTO;

namespace SeatLock.Application.Authentication.Validators;

public sealed class RefreshTokenRequestDTOValidator : AbstractValidator<RefreshTokenRequestDTO>
{
    public RefreshTokenRequestDTOValidator() => RuleFor(value => value.RefreshToken).NotEmpty();
}

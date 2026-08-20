using FluentValidation;
using MediatR;
using SeatLock.Application.Authentication.Commands;
using SeatLock.Application.Authentication.DTO;
using SeatLock.Application.Common.Exceptions;

namespace SeatLock.Application.Authentication.Handlers;

public sealed class LoginCommandHandler(
    IAuthenticationService authenticationService,
    IValidator<LoginRequestDTO> validator) : IRequestHandler<LoginCommand, AuthTokenResultDTO>
{
    public async Task<AuthTokenResultDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request.Request, cancellationToken);
        if (!validation.IsValid)
        {
            throw new RequestValidationException(validation.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray()));
        }

        return await authenticationService.LoginAsync(request.Request, cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");
    }
}

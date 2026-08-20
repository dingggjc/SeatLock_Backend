using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SeatLock.Application.Authentication.Commands;
using SeatLock.Application.Authentication.DTO;
using SeatLock.Application.Common.Exceptions;
using SeatLock.Application.Common.Interfaces;

namespace SeatLock.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMediator mediator, ICurrentUserService currentUser, ITenantContext tenantContext) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    public async Task<ActionResult<AuthTokenResultDTO>> Login([FromBody] LoginRequestDTO model, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginCommand(model), cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("authentication")]
    public async Task<ActionResult<AuthTokenResultDTO>> Refresh([FromBody] RefreshTokenRequestDTO model, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RefreshTokenCommand(model), cancellationToken);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<CurrentUserProfileDTO> Me()
    {
        if (currentUser.UserId is not { } userId || currentUser.Email is not { } email || tenantContext.TenantId is not { } tenantId)
        {
            throw new UnauthorizedException("The access token does not contain the required identity claims.");
        }

        return Ok(new CurrentUserProfileDTO
        {
            UserId = userId,
            Email = email,
            TenantId = tenantId,
            Roles = currentUser.Roles
        });
    }
}

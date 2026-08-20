using MediatR;
using SeatLock.Application.Authentication.DTO;

namespace SeatLock.Application.Authentication.Commands;

public sealed record RefreshTokenCommand(RefreshTokenRequestDTO Request) : IRequest<AuthTokenResultDTO>;

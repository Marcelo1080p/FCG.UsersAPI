using FCG.UsersAPI.Application.Users.Commands.Login;
using FCG.UsersAPI.Application.Users.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCG.UsersAPI.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Register), new { id = result.Value }, null)
            : BadRequest(new { error = result.Error });
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(cmd, ct);
        return result.IsSuccess
            ? Ok(new { token = result.Value })
            : Unauthorized(new { error = result.Error });
    }
}

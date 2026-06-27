using FCG.UsersAPI.Application.Users.Commands.DeactivateUser;
using FCG.UsersAPI.Application.Users.Commands.PromoteUser;
using FCG.UsersAPI.Application.Users.Queries.GetAllUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.UsersAPI.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllUsersQuery(), ct);
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeactivateUserCommand(id), ct);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new { error = result.Error });
    }

    [HttpPatch("{id:guid}/promote")]
    public async Task<IActionResult> Promote(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new PromoteUserCommand(id), ct);
        return result.IsSuccess
            ? Ok(new { message = "Usuário promovido a administrador." })
            : BadRequest(new { error = result.Error });
    }
}

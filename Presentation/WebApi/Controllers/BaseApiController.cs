using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private readonly IMediator _mediator;

    protected IMediator Mediator => _mediator;

    public BaseApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
}

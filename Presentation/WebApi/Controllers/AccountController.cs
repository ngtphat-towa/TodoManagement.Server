using MediatR;

namespace WebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        public AccountController(IMediator mediator) : base(mediator)
        {
        }
    }
}

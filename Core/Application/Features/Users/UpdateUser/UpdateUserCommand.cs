using Application.Interfaces.Repositories;

using MediatR;
using Shared.Wrappers;

namespace Application.Features.Users.UpdateUser
{
    public class UpdateUserCommand: IRequest<Response<Unit>>
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public short Role { get; set; }
    }
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Response<Unit>>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Response<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID '{request.Id}' not found.");
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Role = request.Role;

            await _userService.UpdateAsync(user);

            return Response<Unit>.Success(Unit.Value);
        }
    }
}

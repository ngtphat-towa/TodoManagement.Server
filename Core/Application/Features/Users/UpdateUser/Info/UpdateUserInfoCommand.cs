using Application.Interfaces.Repositories;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.UpdateUser.Info
{
    public class UpdateUserInfoCommand : IRequest<Response<Unit>>
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserInfoCommand, Response<Unit>>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Response<Unit>> Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID '{request.Id}' not found.");
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _userService.UpdateAsync(user);

            return Response<Unit>.Success(Unit.Value);
        }
    }
}

using Application.Interfaces.Repositories;

using Domain.Entities;

using MapsterMapper;

using MediatR;

using Shared.Wrappers;

namespace Application.Features.Users.GetAllUsers;

public class GetAllUsersQuery : PaginationFilter, IRequest<PagedResponse<IEnumerable<User>>>
{
}
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResponse<IEnumerable<User>>>
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    public GetAllUsersQueryHandler(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<PagedResponse<IEnumerable<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var validFilter = _mapper.Map<PaginationFilter>(request);
        var user = await _userService.GetPagedResponseAsync(validFilter.PageNumber, validFilter.PageSize);
        var record = await _userService.GetCountTotalPagedResponseAsync(validFilter.PageNumber, validFilter.PageSize);
        var userViewModel = _mapper.Map<IEnumerable<User>>(user);
        return new PagedResponse<IEnumerable<User>>(userViewModel, validFilter.PageNumber, validFilter.PageSize, record.TotalPages, record.TotalRecords);
    }
}
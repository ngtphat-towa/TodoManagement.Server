using Contracts.Todo;
using Contracts.User;

using Domain.Entities;

using Mapster;

using Newtonsoft.Json;

namespace Application.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        TypeAdapterConfig<User, UserResponse>.NewConfig()
           .Map(dest => dest.Id, src => src.Id)
           .Map(dest => dest.FirstName, src => src.FirstName)
           .Map(dest => dest.LastName, src => src.LastName)
           .Map(dest => dest.Username, src => src.UserName)
           .Map(dest => dest.Email, src => src.Email)
           .Map(dest => dest.Role, src => src.Role)
           .Map(dest => dest.Permissions, src => JsonConvert.DeserializeObject<List<string>>(src.Permissions!));
    }
}

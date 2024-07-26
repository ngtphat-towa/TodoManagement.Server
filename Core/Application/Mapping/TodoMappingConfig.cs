using Application.Features.Todos.CreateTodo;

using Contracts.Todo;

using Domain.Entities;

using Mapster;

namespace Application.Mapping;

public class TodoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateTodoCommand, Todo>()
           .Map(dest => dest.Title, src => src.Title)
           .Map(dest => dest.Description, src => src.Description)
           .Map(dest => dest.Status, src => src.Status);

        TypeAdapterConfig<Todo, TodoResponse>.NewConfig()
     .Map(dest => dest.Id, src => src.Id)
     .Map(dest => dest.Title, src => src.Title)
     .Map(dest => dest.Description, src => src.Description)
     .Map(dest => dest.Description, src => src.Description)
     .Map(dest => dest.Status, src => src.Status)
     // Map other properties accordingly
     .Ignore(dest => dest.CreatedBy)
     .Ignore(dest => dest.Created)
     .Ignore(dest => dest.LastModifiedBy)
     .Ignore(dest => dest.LastModified!);

        config.NewConfig<Todo, TodoResponse>();
    }
}

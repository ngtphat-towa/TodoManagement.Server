using Application.Features.Todos.CreateTodo;

using Domain.Entity;

using Mapster;

namespace Application.Mapping
{
    public class TodoMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateTodoCommand, Todo>()
               .Map(dest => dest.Title, src => src.Title)
               .Map(dest => dest.Description, src => src.Description)
               .Map(dest => dest.Status, src => src.Status);

            config.NewConfig<Todo, CreateTodoCommand>();
        }
    }
}

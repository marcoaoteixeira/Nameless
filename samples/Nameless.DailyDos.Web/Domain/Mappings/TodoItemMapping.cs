using AutoMapper;
using Nameless.DailyDos.Web.Api.v1.Models.Input;
using Nameless.DailyDos.Web.Api.v1.Models.Output;
using Nameless.DailyDos.Web.Domain.Dtos;
using Nameless.DailyDos.Web.Domain.Entities;
using Nameless.DailyDos.Web.Domain.Requests;

namespace Nameless.DailyDos.Web.Domain.Mappings {
    public sealed class TodoItemMapping : Profile {
        #region Public Constructors

        public TodoItemMapping() {
            CreateMap<TodoItem, TodoItemDto>();
            CreateMap<CreateTodoItemRequest, TodoItem>();
            CreateMap<CreateTodoItemInput, CreateTodoItemRequest>();
            CreateMap<TodoItemDto, CreateTodoItemOutput>();
        }

        #endregion
    }
}

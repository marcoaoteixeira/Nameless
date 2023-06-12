using AutoMapper;
using Nameless.WebApplication.Domain.v1.Users.Commands;
using Nameless.WebApplication.Domain.v1.Users.Models.Input;

namespace Nameless.WebApplication.Domain.v1.Users.Mappings {

    public sealed class CreateUserInput_CreateUserCommand : Profile {

        #region Public Constructors

        public CreateUserInput_CreateUserCommand() {
            CreateMap<CreateUserInput, CreateUserCommand>();
        }

        #endregion
    }
}

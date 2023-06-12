using AutoMapper;
using Nameless.WebApplication.Domain.v1.Users.Models.Output;
using Nameless.WebApplication.Entities;

namespace Nameless.WebApplication.Domain.v1.Users.Mappings {

    public sealed class User_UserOutput : Profile {

        #region Public Constructors

        public User_UserOutput() {
            CreateMap<User, UserOutput>();
        }

        #endregion
    }
}

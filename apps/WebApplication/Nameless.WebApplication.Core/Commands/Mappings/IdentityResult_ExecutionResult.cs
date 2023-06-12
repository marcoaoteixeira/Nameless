using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Nameless.Infrastructure;

namespace Nameless.WebApplication.Commands.Mappings {

    public sealed class IdentityResult_ExecutionResult : Profile {

        #region Public Constructors

        public IdentityResult_ExecutionResult() {
            CreateMap<IdentityError, ExecutionResult.Error>()
                .ConstructUsing(src => new ExecutionResult.Error(src.Code, src.Description));

            CreateMap<IdentityResult, ExecutionResult>()
                .ConstructUsing((src, ctx) => ExecutionResult.Failure(errors: ctx.Mapper.Map<ExecutionResult.Error[]>(src.Errors)));
        }

        #endregion
    }
}

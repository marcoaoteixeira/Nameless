using AutoMapper;
using FluentValidation.Results;
using Nameless.Infrastructure;

namespace Nameless.CommandQuery.Mappings
{

    public sealed class ValidationResult_ExecutionResult : Profile {

        #region Public Constructors

        public ValidationResult_ExecutionResult() {
            CreateMap<ValidationFailure, ExecutionResult.Error>()
                .ConstructUsing(src => new ExecutionResult.Error(src.PropertyName, src.ErrorMessage));

            CreateMap<ValidationResult, ExecutionResult>()
                .ConstructUsing((src, ctx) => ExecutionResult.Failure(ctx.Mapper.Map<ExecutionResult.Error[]>(src.Errors)));
        }

        #endregion
    }
}

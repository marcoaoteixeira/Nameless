using AutoMapper;
using FluentValidation;
using Nameless.CommandQuery;
using Nameless.WebApplication.Entities;

namespace Nameless.WebApplication.Commands {

    public abstract class DbContextCommandHandlerBase<TCommand> : CommandHandlerBase<TCommand>
        where TCommand : ICommand {

        #region Protected Properties

        protected ApplicationDbContext DbContext { get; }

        #endregion

        #region Protected Constructors

        protected DbContextCommandHandlerBase(ApplicationDbContext dbContext, IMapper mapper, IValidator<TCommand>? validator = default)
            : base(mapper, validator) {
            Prevent.Null(dbContext, nameof(dbContext));

            DbContext = dbContext;
        }

        #endregion
    }
}

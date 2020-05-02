using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.Bookshelf.Models;
using Nameless.CQRS;
using NHibernate;

namespace Nameless.Bookshelf.Commands {
    public sealed class CreateAuthorCommand : ICommand {
        #region Public Properties

        public Guid ID { get; internal set; }
        public string Name { get; set; }

        #endregion
    }

    public sealed class CreateAuthorCommandHandler : CommandHandlerBase<CreateAuthorCommand> {
        #region Public Constructors

        public CreateAuthorCommandHandler (ISession session) : base (session) { }

        #endregion

        #region Public Override Methods

        public override Task HandleAsync (CreateAuthorCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var now = DateTime.Now.Date;

            command.ID = Guid.NewGuid ();

            var author = new Author {
                ID = command.ID,
                Name = command.Name,
                CreationDate = now,
                ModificationDate = null
            };

            return Session.SaveAsync (author, token);
        }

        #endregion
    }
}
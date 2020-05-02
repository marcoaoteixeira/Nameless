using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.Bookshelf.Models;
using Nameless.CQRS;
using NHibernate;
using NHibernate.Criterion;

namespace Nameless.Bookshelf.Commands {
    public sealed class UpdateAuthorCommand : ICommand {
        #region Public Properties

        public Guid ID { get; set; }
        public string Name { get; set; }

        #endregion
    }

    public sealed class UpdateAuthorCommandHandler : CommandHandlerBase<UpdateAuthorCommand> {
        #region Public Constructors

        public UpdateAuthorCommandHandler (ISession session) : base (session) { }

        #endregion

        #region Public Override Methods

        public override Task HandleAsync (UpdateAuthorCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var criteria = Session.CreateCriteria<Author> ();

            criteria
                .Add (Restrictions.Eq (nameof (Author.ID), command.ID));

            var author = criteria.UniqueResult<Author> ();

            author.Name = command.Name;
            author.ModificationDate = DateTime.Now.Date;

            return Session.UpdateAsync (author, token);
        }

        #endregion
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.Bookshelf.Models;
using Nameless.CQRS;
using NHibernate;
using NHibernate.Criterion;

namespace Nameless.Bookshelf.Queries {
    public sealed class GetAuthorQuery : IQuery<Author> {
        #region Public Properties

        public Guid ID { get; set; }

        #endregion
    }

    public sealed class GetAuthorQueryHandler : QueryHandlerBase<GetAuthorQuery, Author> {
        #region Public Constructors

        public GetAuthorQueryHandler (ISession session) : base (session) { }

        #endregion

        #region Public Override Methods

        public override Task<Author> HandleAsync (GetAuthorQuery query, IProgress<int> progress = null, CancellationToken token = default) {
            var criteria = Session.CreateCriteria<Author> ();

            criteria
                .Add (Restrictions.Eq (nameof (Author.ID), query.ID));

            return criteria.UniqueResultAsync<Author> (token);
        }

        #endregion
    }
}
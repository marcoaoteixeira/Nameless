using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nameless.Bookshelf.Models;
using Nameless.CQRS;
using NHibernate;
using NHibernate.Criterion;

namespace Nameless.Bookshelf.Queries {
    public sealed class BookSearchQuery : IQuery<IList<Book>> {
        #region Public Properties

        public string Title { get; set; }
        public Guid? OwnerID { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;

        #endregion
    }

    public sealed class BookSearchQueryHandler : QueryHandlerBase<BookSearchQuery, IList<Book>> {
        #region Public Constructors

        public BookSearchQueryHandler (ISession session) : base (session) { }

        #endregion

        #region Public Override Methods

        public override Task<IList<Book>> HandleAsync (BookSearchQuery query, IProgress<int> progress = null, CancellationToken token = default) {
            var criteria = Session.CreateCriteria<Book> ();

            if (!string.IsNullOrWhiteSpace (query.Title)) {
                criteria
                    .Add (Restrictions.Like (nameof (Book.Title), query.Title));
            }

            if (query.OwnerID.HasValue) {
                criteria
                    .Add (Restrictions.Eq (nameof (Book.OwnerID), query.OwnerID.Value));
            }

            criteria
                .SetFirstResult (query.PageIndex * query.PageSize)
                .SetMaxResults (query.PageSize);

            return criteria.ListAsync<Book> (token);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Microsoft.Extensions.FileProviders;
using Nameless.Bookshelf.Models;
using Nameless.CQRS;
using Nameless.Data;
using Nameless.FileProvider.Common;

namespace Nameless.Bookshelf.Queries {
    public sealed class BookSearchQuery : IQuery<IEnumerable<Book>> {
        #region Public Properties

        public string Title { get; set; }
        public Guid? OwnerID { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;

        #endregion
    }

    public sealed class BookSearchQueryHandler : QueryHandlerBase<BookSearchQuery, IEnumerable<Book>> {
        #region Public Constructors

        public BookSearchQueryHandler (IDatabase database, IFileProvider fileProvider) : base (database, fileProvider) { }

        #endregion

        #region Public Override Methods

        public override async Task<IEnumerable<Book>> HandleAsync (BookSearchQuery query, IProgress<int> progress = null, CancellationToken token = default) {
            var sql = FileProvider.GetFileInfo ($"{ResourcePath}/SearchBooks.sql").GetText ();

            var books = await Database.ExecuteReaderAsync (
                commandText: sql,
                mapper: Book.Map,
                parameters: new [] {
                    Parameter.CreateInputParameter (nameof (Book.Title), query.Title),
                    Parameter.CreateInputParameter (nameof (Book.OwnerID), query.OwnerID, DbType.Guid),
                    Parameter.CreateInputParameter (nameof (query.PageIndex), query.PageIndex, DbType.Int32),
                    Parameter.CreateInputParameter (nameof (query.PageSize), query.PageSize, DbType.Int32)
                }
            ).ToArrayAsync (token);

            return new List<Book> (books);
        }

        #endregion
    }
}
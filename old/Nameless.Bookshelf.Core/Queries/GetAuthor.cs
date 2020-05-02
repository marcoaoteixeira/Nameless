using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Nameless.Bookshelf.Models;
using Nameless.CQRS;
using Nameless.Data;
using Nameless.FileProvider.Common;

namespace Nameless.Bookshelf.Queries {
    public sealed class GetAuthorQuery : IQuery<Author> {
        #region Public Properties

        public Guid ID { get; set; }

        #endregion
    }

    public sealed class GetAuthorQueryHandler : QueryHandlerBase<GetAuthorQuery, Author> {
        #region Public Constructors

        public GetAuthorQueryHandler (IDatabase database, IFileProvider fileProvider)
            : base (database, fileProvider) { }

        #endregion

        #region Public Override Methods

        public override Task<Author> HandleAsync (GetAuthorQuery query, IProgress<int> progress = null, CancellationToken token = default) {
            var sql = FileProvider.GetFileInfo ($"{ResourcePath}/GetAuthor.sql").GetText ();

            return Database.ExecuteReaderSingleAsync (
                commandText: sql,
                mapper: Author.Map,
                parameters: new[] {
                    Parameter.CreateInputParameter (nameof (Author.ID), query.ID, DbType.Guid)
                }
            );
        }

        #endregion
    }
}

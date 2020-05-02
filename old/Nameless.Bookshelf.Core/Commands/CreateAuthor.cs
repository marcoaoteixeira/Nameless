using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Nameless.Bookshelf.Models;
using Nameless.CQRS;
using Nameless.Data;
using Nameless.FileProvider.Common;

namespace Nameless.Bookshelf.Commands {
    public sealed class CreateAuthorCommand : ICommand {
        #region Public Properties

        public Guid ID { get; internal set; }
        public string Name { get; set; }

        #endregion
    }

    public sealed class CreateAuthorCommandHandler : CommandHandlerBase<CreateAuthorCommand> {
        #region Public Constructors

        public CreateAuthorCommandHandler (IDatabase database, IFileProvider fileProvider)
            : base (database, fileProvider) { }

        #endregion

        #region Public Override Methods

        public override Task HandleAsync (CreateAuthorCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var sql = FileProvider.GetFileInfo ($"{ResourcePath}/CreateAuthor.sql").GetText ();
            var now = DateTime.Now.Date;

            command.ID = Guid.NewGuid ();
            
            return Database.ExecuteNonQueryAsync (sql, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Author.ID), command.ID, DbType.Guid),
                Parameter.CreateInputParameter (nameof (Author.Name), command.Name),
                Parameter.CreateInputParameter (nameof (Author.CreationDate), now, DbType.DateTime)
            });
        }

        #endregion
    }
}

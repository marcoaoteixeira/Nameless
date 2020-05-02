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
    public sealed class UpdateAuthorCommand : ICommand {
        #region Public Properties

        public Guid ID { get; set; }
        public string Name { get; set; }

        #endregion
    }

    public sealed class UpdateAuthorCommandHandler : CommandHandlerBase<UpdateAuthorCommand> {
        #region Public Constructors

        public UpdateAuthorCommandHandler (IDatabase database, IFileProvider fileProvider)
            : base (database, fileProvider) { }

        #endregion

        #region Public Override Methods

        public override Task HandleAsync (UpdateAuthorCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var sql = FileProvider.GetFileInfo ($"{ResourcePath}/UpdateAuthor.sql").GetText ();
            var now = DateTime.Now.Date;

            command.ID = Guid.NewGuid ();
            
            return Database.ExecuteNonQueryAsync (sql, parameters: new[] {
                Parameter.CreateInputParameter (nameof (Author.ID), command.ID, DbType.Guid),
                Parameter.CreateInputParameter (nameof (Author.Name), command.Name),
                Parameter.CreateInputParameter (nameof (Author.ModificationDate), now, DbType.DateTime)
            });
        }

        #endregion
    }
}

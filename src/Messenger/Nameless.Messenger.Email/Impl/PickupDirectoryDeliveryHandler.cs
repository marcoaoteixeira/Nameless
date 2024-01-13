using MimeKit;
using Nameless.Infrastructure;

namespace Nameless.Messenger.Email.Impl {
    public sealed class PickupDirectoryDeliveryHandler : IDeliveryHandler {
        #region Private Read-Only Fields

        private readonly IApplicationContext _applicationContext;
        private readonly MessengerOptions _options;
        private readonly FileNameGeneratorDelegate _fileNameGenerator;

        #endregion

        #region Public Constructors

        public PickupDirectoryDeliveryHandler(IApplicationContext applicationContext)
            : this(applicationContext, Root.Defaults.FileNameGeneratorFactory, MessengerOptions.Default) { }

        public PickupDirectoryDeliveryHandler(IApplicationContext applicationContext, FileNameGeneratorDelegate fileNameGenerator)
            : this(applicationContext, fileNameGenerator, MessengerOptions.Default) { }

        public PickupDirectoryDeliveryHandler(IApplicationContext applicationContext, FileNameGeneratorDelegate fileNameGenerator, MessengerOptions options) {
            _applicationContext = Guard.Against.Null(applicationContext, nameof(applicationContext));
            _fileNameGenerator = Guard.Against.Null(fileNameGenerator, nameof(fileNameGenerator));
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Public Methods

        public string GetPickupDirectoryPath()
            => Path.Combine(
                _applicationContext.ApplicationDataFolderPath,
                _options.PickupDirectoryFolderName
            );

        #endregion

        #region Private Methods

        private string GetFilePath()
            => Path.Combine(
                GetPickupDirectoryPath(),
                _fileNameGenerator()
            );

        #endregion

        #region IDeliveryHandler Members

        public DeliveryMode Mode => DeliveryMode.PickupDirectory;

        public async Task HandleAsync(MimeMessage message, CancellationToken cancellationToken = default) {
            var path = GetFilePath();

            using var stream = new FileStream(path, FileMode.Create);
            await message.WriteToAsync(
                stream,
                headersOnly: false,
                cancellationToken
            );
        }

        #endregion
    }
}

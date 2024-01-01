using MimeKit;
using Nameless.Infrastructure;

namespace Nameless.Messenger.Email.Impl {
    public sealed class PickupDirectoryDeliveryHandler : IDeliveryHandler {
        #region Public Static Read-Only Fields

        public static Func<string> DefaultFileNameGenerator => Path.GetRandomFileName;

        #endregion

        #region Private Read-Only Fields

        private readonly IApplicationContext _applicationContext;
        private readonly MessengerOptions _options;
        private readonly Func<string> _fileNameGenerator;

        #endregion

        #region Public Constructors

        public PickupDirectoryDeliveryHandler(IApplicationContext applicationContext, MessengerOptions? options = null, Func<string>? fileNameGenerator = null) {
            _applicationContext = Guard.Against.Null(applicationContext, nameof(applicationContext));
            _options = options ?? MessengerOptions.Default;
            _fileNameGenerator = fileNameGenerator ?? DefaultFileNameGenerator;
        }

        #endregion

        #region Public Methods

        public string GetPickupDirectory()
            => Path.Combine(
                _applicationContext.BasePath,
                _options.PickupDirectoryFolder
            );

        #endregion

        #region Private Methods

        private string GetFilePath()
            => Path.Combine(
                GetPickupDirectory(),
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

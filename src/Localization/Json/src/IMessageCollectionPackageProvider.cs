using System.Threading;
using System.Threading.Tasks;
using Nameless.Localization.Json.Schemas;

namespace Nameless.Localization.Json {
    public interface IMessageCollectionPackageProvider {
        #region Methods

        Task<MessageCollectionPackage> CreateAsync (string cultureName, CancellationToken token = default);

        #endregion
    }
}
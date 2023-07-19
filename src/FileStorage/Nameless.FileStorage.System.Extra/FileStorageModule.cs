using Autofac;
using Nameless.Autofac;

namespace Nameless.FileStorage.System {
    public sealed class FileStorageModule : ModuleBase {
        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterType<FileStorageImpl>()
                .As<IFileStorage>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion
    }
}

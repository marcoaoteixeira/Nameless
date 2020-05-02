namespace Nameless.FileProvider {
    public interface IFileProvider {
        #region Properties

        FileProviderType Type { get; }

        #endregion

        #region Methods

        IDirectory GetDirectory (string path);
        IFile GetFile (string path);
        IWatchToken Watch (string filter);

        #endregion
    }
}
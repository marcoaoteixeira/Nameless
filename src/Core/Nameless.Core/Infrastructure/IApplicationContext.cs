namespace Nameless.Infrastructure {

    public interface IApplicationContext {

        #region Properties

        string EnvironmentName { get; }
        string ApplicationName { get; }
        string BasePath { get; }
        string DataDirectoryPath { get; }

        #endregion
    }
}

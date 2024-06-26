﻿namespace Nameless.MongoDB.Options {
    public sealed class MongoOptions {
        #region Public Static Read-Only Properties

        public static MongoOptions Default => new();

        #endregion

        #region Private Fields

        private CredentialsOptions? _credentials;

        #endregion

        #region Public Properties

        public string Host { get; set; } = Root.Defaults.MONGO_HOST;
        public int Port { get; set; } = Root.Defaults.MONGO_PORT;
        public string? Database { get; set; }
        public CredentialsOptions Credentials {
            get => _credentials ??= CredentialsOptions.Default;
            set => _credentials = value;
        }

        public ICollectionNamingStrategy CollectionNamingStrategy { get; private set; } = Impl.CollectionNamingStrategy.Instance;

        public Type[] ClassMappingTypes { get; private set; } = [];

        #endregion

        #region Public Methods

        public void SetCollectionNamingStrategy(ICollectionNamingStrategy collectionNamingStrategy)
            => CollectionNamingStrategy = collectionNamingStrategy;

        public void SetClassMappingTypes(params Type[] classMappingTypes)
            => ClassMappingTypes = classMappingTypes;

        #endregion
    }
}

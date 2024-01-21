using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.MongoDB.DependencyInjection {
    public sealed class Bootstrapper : IStartable {
        #region Private Read-Only Fields

        private readonly ILogger _logger;
        private readonly Type[] _mappings;

        #endregion

        #region Public Constructors

        public Bootstrapper(Type[] mappings)
            : this(mappings, NullLogger.Instance) { }

        public Bootstrapper(Type[] mappings, ILogger logger) {
            _mappings = mappings ?? [];
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region IStartable Members

        public void Start() {
            foreach (var mapping in _mappings) {
                try { Activator.CreateInstance(mapping); } catch (Exception ex) {
                    _logger.LogError(
                        exception: ex,
                        message: "Error while running mapping {mapping}",
                        args: mapping
                    );
                }
            }
        }

        #endregion
    }
}

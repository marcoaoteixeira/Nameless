namespace Nameless.Logging.log4net.UnitTests.Fixtures {

    public class ServiceWithLogger : IService {

        public ILogger Log { get; set; } = NullLogger.Instance;

        public string Concat(string first, string second) {
            Log.Debug($"Parameters first: {first}, second: {second}");

            return string.Concat(first, second);
        }
    }
}

using System.Reflection;
using Autofac;
using Nameless.Autofac;

namespace Nameless.CommandQuery {
    public sealed class CommandQueryModule : ModuleBase {
        #region Public Properties

        public Type[] CommandHandlerImplementations { get; set; } = Array.Empty<Type>();

        public Type[] QueryHandlerImplementations { get; set; } = Array.Empty<Type>();

        #endregion

        #region Public Constructors

        public CommandQueryModule(params Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterType<CommandService>()
                .As<ICommandService>()
                .SingleInstance();

            builder
                .RegisterType<QueryService>()
                .As<IQueryService>()
                .SingleInstance();

            var commandHandlerImplementations = CommandHandlerImplementations.IsNullOrEmpty()
                ? SearchForImplementations(typeof(ICommandHandler<>))
                : CommandHandlerImplementations;
            builder
                .RegisterTypes(commandHandlerImplementations)
                .AsClosedTypesOf(typeof(ICommandHandler<>))
                .SingleInstance();

            var queryHandlerImplementations = QueryHandlerImplementations.IsNullOrEmpty()
                ? SearchForImplementations(typeof(IQueryHandler<,>))
                : QueryHandlerImplementations;
            builder
                .RegisterTypes(queryHandlerImplementations)
                .AsClosedTypesOf(typeof(IQueryHandler<,>))
                .SingleInstance();

            var queryImplementations = FetchQueries(queryHandlerImplementations).ToArray();
            builder
                .RegisterTypes(queryImplementations)
                .AsClosedTypesOf(typeof(IQuery<>))
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IEnumerable<Type> FetchQueries(Type[] queryHandlers) {
            foreach (var queryHandler in queryHandlers) {
                var @interface = queryHandler.GetInterfaces().FirstOrDefault(_ => typeof(IQueryHandler<,>).IsAssignableFromGenericType(_));
                if (@interface != default) {
                    var query = @interface.GetGenericArguments().FirstOrDefault(_ => typeof(IQuery<>).IsAssignableFromGenericType(_));
                    if (query != default) {
                        yield return query;
                    }
                }
            }
        }

        #endregion
    }
}

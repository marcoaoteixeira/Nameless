namespace Nameless.Generators.EventSourcing;

internal static class EventSourcingConstants {
    internal static class Project {
        internal const string Name = "Nameless Event Sourcing Generator";
        
        internal static class Namespaces {
            internal const string Root = "Nameless.EventSourcing";
        }

        internal static class Classes {
            internal static class Names {
                internal const string AggregateRoot = "AggregateRoot";
            }

            internal static class FullNames {
                internal const string AggregateRoot = $"{Namespaces.Root}.AggregateRoot`1";
            }
        }
    }

    internal static class AggregateRootClass {
        internal const string FullName = $"{Project.Namespaces.Root}.AggregateRoot";
        internal const string FullNameWithArity = $"{Project.Namespaces.Root}.AggregateRoot`1";

        internal const string WhenMethodName = "When";
    }
    
    // Fully Qualified Names
    internal static class FQN {
        internal const string EventInterface = "global::Nameless.Mediator.Events.IEvent";
    }
}

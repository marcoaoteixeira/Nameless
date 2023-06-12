using AutoMapper;

namespace Nameless.CommandQuery.UnitTests.Fixtures {

    public sealed class ListAnimalQuery : IQuery<Animal[]> {

    }

    public sealed class ListAnimalQueryHandler : QueryHandlerBase<ListAnimalQuery, Animal[]> {
        public ListAnimalQueryHandler(IMapper mapper) : base(mapper) {
        }

        public override Task<Animal[]> HandleAsync(ListAnimalQuery query, CancellationToken cancellationToken = default) {
            return Task.FromResult(new[] {
                new Animal {
                    ID = 1,
                    Name = "Test"
                }
            });
        }
    }
}

namespace Nameless.CommandQuery.UnitTests.Fixtures {

    public sealed class GetAnimalQuery : IQuery<Animal> {

        public int ID { get; set; }
    }

    public sealed class GetAnimalQueryHandler : IQueryHandler<GetAnimalQuery, Animal> {
        public Task<Animal> HandleAsync(GetAnimalQuery query, CancellationToken cancellationToken = default) {
            return Task.FromResult(new Animal { ID = query.ID, Name = "Test" });
        }
    }
}

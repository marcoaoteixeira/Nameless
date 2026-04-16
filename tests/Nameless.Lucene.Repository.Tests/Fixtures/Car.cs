using Bogus;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository.Fixtures;

public class Car {
    public Guid ID { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
}

public class CarEntityMapping : IEntityMapping<Car> {
    public static IEntityMapping<Car> Instance { get; } = new CarEntityMapping();

    public void Map(IEntityDescriptor<Car> descriptor) {
        descriptor
            .SetID(expr => expr.ID)
            .Property(expr => expr.Brand)
            .Property(expr => expr.Model)
            .Property(expr => expr.Year);
    }
}

public class MissingIDCarEntityMapping : IEntityMapping<Car> {
    public static IEntityMapping<Car> Instance { get; } = new MissingIDCarEntityMapping();

    public void Map(IEntityDescriptor<Car> descriptor) {
        descriptor
            .Property(expr => expr.Brand)
            .Property(expr => expr.Model)
            .Property(expr => expr.Year);
    }
}

public class CarFaker : Faker<Car> {
    public static CarFaker Instance { get; } = new();

    public CarFaker() {
        base
            .StrictMode(ensureRulesForAllProperties: true)
            .RuleFor(prop => prop.ID, _ => Guid.CreateVersion7())
            .RuleFor(prop => prop.Brand, faker => faker.Vehicle.Manufacturer())
            .RuleFor(prop => prop.Model, faker => faker.Vehicle.Model())
            .RuleFor(prop => prop.Year, faker => faker.Random.Int(min: 1950, max: 2000));
    }
}

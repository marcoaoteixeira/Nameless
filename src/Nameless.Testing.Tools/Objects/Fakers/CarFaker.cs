using Bogus;
using Nameless.Fixtures;

namespace Nameless.Testing.Tools.Objects.Fakers;

public class CarFaker : Faker<Car> {
    private CarFaker() {
        base.StrictMode(true)
            .RuleFor(prop => prop.ID, faker => faker.IndexFaker)
            .RuleFor(prop => prop.Brand, faker => faker.Vehicle.Manufacturer())
            .RuleFor(prop => prop.Model, faker => faker.Vehicle.Model());
    }

    public static CarFaker Create() {
        return new CarFaker();
    }
}
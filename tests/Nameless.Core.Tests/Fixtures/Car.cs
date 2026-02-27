using Bogus;

namespace Nameless.Fixtures;

public record Car {
    public int ID { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
}

public class CarFaker : Faker<Car> {
    public static CarFaker Instance { get; } = new();

    private int ID { get; set; }

    static CarFaker() { }

    private CarFaker() {
        base.StrictMode(true)
            .RuleFor(prop => prop.ID, _ => ID++)
            .RuleFor(prop => prop.Brand, faker => faker.Vehicle.Manufacturer())
            .RuleFor(prop => prop.Model, faker => faker.Vehicle.Model());
    }

    public void ResetID() {
        ID = 0;
    }
}

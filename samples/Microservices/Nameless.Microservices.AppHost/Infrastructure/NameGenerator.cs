namespace Nameless.Microservices.AppHost.Infrastructure;

internal static class NameGenerator {
    private static readonly Random Rnd = new();

    private static readonly string[] Adjectives =
    [
        "Audacious", "Brave", "Cosmic", "Daring", "Elegant",
        "Fierce", "Groovy", "Hardy", "Intrepid", "Jovial",
        "Kinetic", "Lunar", "Mantic", "Noble", "Oracular",
        "Plucky", "Quixotic", "Radical", "Serene", "Trusty",
        "Utopic", "Valiant", "Warty", "Xenial", "Zealous"
    ];

    private static readonly string[] Animals =
    [
        "Axolotl", "Bison", "Camel", "Dingo", "Echidna",
        "Fennec", "Gibbon", "Heron", "Ibex", "Jackal",
        "Koala", "Lynx", "Meerkat", "Narwhal", "Ocelot",
        "Pangolin", "Quokka", "Raven", "Salamander", "Tapir",
        "Urial", "Viper", "Wombat", "Xerus", "Yak"
    ];

    internal static string Generate(string separator = " ") {
        var adjective = Adjectives[Rnd.Next(Adjectives.Length)];
        var animal = Animals[Rnd.Next(Animals.Length)];

        return $"{adjective}{separator}{animal}";
    }
}

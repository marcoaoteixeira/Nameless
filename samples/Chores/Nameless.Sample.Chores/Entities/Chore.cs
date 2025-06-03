namespace Nameless.Sample.Chores.Entities;

public class Chore {
    public required Guid ID { get; init; }

    public required string Description { get; set; }

    public bool Done { get; set; }

    public required DateOnly Date { get; set; }

    public bool Equals(Chore? obj) {
        return obj is not null && ID.Equals(obj.ID);
    }

    public override bool Equals(object? obj) {
        return Equals(obj as Chore);
    }

    public override int GetHashCode() {
        return ID.GetHashCode();
    }
}

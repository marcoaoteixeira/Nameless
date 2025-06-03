using Nameless.Sample.Chores.Dtos;

namespace Nameless.Sample.Chores.Entities;

public static class ChoreExtensions {
    public static ChoreDto ToDto(this Chore entity) {
        return new ChoreDto {
            ID = entity.ID,
            Description = entity.Description,
            Done = entity.Done,
            Date = entity.Date
        };
    }
}

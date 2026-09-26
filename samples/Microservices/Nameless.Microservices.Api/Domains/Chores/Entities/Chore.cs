using System.ComponentModel.DataAnnotations;

namespace Nameless.Microservices.Api.Domains.Chores.Entities;

public class Chore {
    public Guid ID { get; set; }

    [MaxLength(256)]
    public required string Title { get; set; }

    [MaxLength(4096)]
    public required string Description { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset? ConclusionDate { get; set; }
}

internal static class ChoreQueryExtensions {
    extension(IQueryable<Chore> self) {
        internal IQueryable<Chore> WithinTitle(string? value, bool ignoreCase = true) {
            if (string.IsNullOrWhiteSpace(value)) { return self; }

            return self.Where(
                chore => chore.Title.Contains(
                    value,
                    comparisonType: ignoreCase
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal
                )
            );
        }

        internal IQueryable<Chore> WithinDescription(string? value, bool ignoreCase = true) {
            if (string.IsNullOrWhiteSpace(value)) { return self; }

            return self.Where(
                chore => chore.Description.Contains(
                    value,
                    comparisonType: ignoreCase
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal
                )
            );
        }

        internal IQueryable<Chore> BetweenDueDate(DateTimeOffset? start, DateTimeOffset? end) {
            if (!start.HasValue && !end.HasValue) { return self; }

            if (start.HasValue && !end.HasValue) {
                return self.Where(chore => chore.DueDate >= start.Value);
            }

            if (!start.HasValue && end.HasValue) {
                return self.Where(chore => chore.DueDate <= end.Value);
            }

            return self.Where(
                chore => chore.DueDate >= start.GetValueOrDefault() &&
                         chore.DueDate <= end.GetValueOrDefault()
            );
        }

        internal IQueryable<Chore> BetweenConclusionDate(DateTimeOffset? start, DateTimeOffset? end) {
            if (!start.HasValue && !end.HasValue) { return self; }

            if (start.HasValue && !end.HasValue) {
                return self.Where(chore => chore.ConclusionDate >= start.Value);
            }

            if (!start.HasValue && end.HasValue) {
                return self.Where(chore => chore.ConclusionDate <= end.Value);
            }

            return self.Where(
                chore => chore.ConclusionDate >= start.GetValueOrDefault() &&
                         chore.ConclusionDate <= end.GetValueOrDefault()
            );
        }

        internal IQueryable<Chore> IsDone(bool? done) {
            return done is true
                ? self.Where(chore => chore.ConclusionDate != null)
                : self;
        }
    }
}
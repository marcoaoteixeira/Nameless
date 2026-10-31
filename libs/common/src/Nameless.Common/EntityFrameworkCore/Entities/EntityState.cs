namespace Nameless.EntityFrameworkCore.Entities;

/// <summary>
///     Represents the possible states for an entity.
/// </summary>
public enum EntityState {
    /// <summary>
    ///     Entity was persisted.
    /// </summary>
    Saved,

    /// <summary>
    ///     Still editing or draft.
    /// </summary>
    Dirty,

    /// <summary>
    ///     Soft-deleted entity.
    /// </summary>
    Deleted
}
﻿namespace Nameless.Search;

/// <summary>
/// Enumerator for indexable types.
/// </summary>
public enum IndexableType {
    /// <summary>
    /// For <see cref="bool"/> values.
    /// </summary>
    Boolean,
    /// <summary>
    /// For <see cref="string"/> values.
    /// </summary>
    String,
    /// <summary>
    /// For <see cref="byte"/> values.
    /// </summary>
    Byte,
    /// <summary>
    /// For <see cref="short"/> values.
    /// </summary>
    Short,
    /// <summary>
    /// For <see cref="int"/> values.
    /// </summary>
    Integer,
    /// <summary>
    /// For <see cref="long"/> values.
    /// </summary>
    Long,
    /// <summary>
    /// For <see cref="float"/> values.
    /// </summary>
    Float,
    /// <summary>
    /// For <see cref="double"/> values.
    /// </summary>
    Double,
    /// <summary>
    /// For <see cref="System.DateTimeOffset"/> values.
    /// </summary>
    DateTimeOffset,
    /// <summary>
    /// For <see cref="System.DateTime"/> values.
    /// </summary>
    DateTime
}
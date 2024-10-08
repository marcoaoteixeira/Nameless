﻿using Lucene_Field = Lucene.Net.Documents.Field;

namespace Nameless.Lucene.Internals;

internal sealed record Descriptor {
    internal string Name { get; }
    internal object Value { get; }
    internal Lucene_Field.Store Store { get; }
    internal bool Analyze { get; }
    internal bool Sanitize { get; }

    internal Descriptor(string name, object value, Lucene_Field.Store store, bool analyze, bool sanitize) {
        Name = name;
        Value = value;
        Store = store;
        Analyze = analyze;
        Sanitize = sanitize;
    }

    internal static Descriptor Create(Field item)
        => new(
            name: item.Name,
            value: item.Value,
            store: item.Options.HasFlag(FieldOptions.Store)
                ? Lucene_Field.Store.YES
                : Lucene_Field.Store.NO,
            analyze: item.Options.HasFlag(FieldOptions.Analyze),
            sanitize: item.Options.HasFlag(FieldOptions.Sanitize)
        );
}

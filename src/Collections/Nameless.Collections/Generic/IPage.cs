namespace Nameless.Collections.Generic;

public interface IPage<out TItem> {
    int Number { get; }
    int Size { get; }
    int Count { get; }
    TItem[] Items { get; }
}
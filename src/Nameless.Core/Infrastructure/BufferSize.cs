namespace Nameless.Infrastructure;

public enum BufferSize : int {
    /// <summary>
    /// 1Kb
    /// </summary>
    Micro = 1024,

    /// <summary>
    /// 4Kb
    /// </summary>
    Tiny = 1024 * 4,

    /// <summary>
    /// 16Kb
    /// </summary>
    Small = 1024 * 16,

    /// <summary>
    /// 32Kb
    /// </summary>
    Medium = 1024 * 32,

    /// <summary>
    /// 64Kb
    /// </summary>
    Big = 1024 * 64,

    /// <summary>
    /// 1Mb
    /// </summary>
    Huge = 1024 * 1024,

    /// <summary>
    /// 16Mb
    /// </summary>
    Humongous = 1024 * 1024 * 16,
}
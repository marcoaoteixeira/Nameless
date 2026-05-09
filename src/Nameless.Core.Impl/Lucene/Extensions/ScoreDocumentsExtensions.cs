using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene;

/// <summary>
///     <see cref="IEnumerable{T}"/> of <see cref="ScoreDocument"/> extension methods.
/// </summary>
public static class ScoreDocumentsExtensions {
    /// <param name="self">The current <see cref="IEnumerable{T}"/> of <see cref="ScoreDocument"/> instance.</param>
    extension(IEnumerable<ScoreDocument> self) {
        /// <summary>
        ///     Creates a <see cref="Queue{T}"/> from the score document sequence.
        /// </summary>
        /// <returns>
        ///     A <see cref="Queue{T}"/> containing all <see cref="ScoreDocument"/> elements.
        /// </returns>
        public Queue<ScoreDocument> CreateQueue() {
            return new Queue<ScoreDocument>(self);
        }
    }
}
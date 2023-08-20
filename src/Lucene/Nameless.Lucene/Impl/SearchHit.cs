using Lucene.Net.Documents;
using Lucene_Document = Lucene.Net.Documents.Document;

namespace Nameless.Lucene.Impl {
    /// <summary>
    /// Default implementation of <see cref="ISearchHit"/>.
    /// </summary>
    public sealed class SearchHit : ISearchHit {
        #region Private Read-Only Fields

        private readonly Lucene_Document _document;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="SearchHit"/>
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="score">The score.</param>
        public SearchHit(Lucene_Document document, float score) {
            _document = Guard.Against.Null(document, nameof(document));
            Score = score;
        }

        #endregion

        #region ISearchHit Members

        /// <inheritdoc />
        public string DocumentID
            => GetString(nameof(ISearchHit.DocumentID));

        /// <inheritdoc />
        public float Score { get; }

        /// <inheritdoc />
        public int GetInt(string name) {
            var field = _document.GetField(name);

            return field is not null ? int.Parse(field.GetStringValue()) : 0;
        }

        /// <inheritdoc />
        public double GetDouble(string name) {
            var field = _document.GetField(name);

            return field is not null ? double.Parse(field.GetStringValue()) : 0d;
        }

        /// <inheritdoc />
        public bool GetBoolean(string name) => GetInt(name) > 0;

        /// <inheritdoc />
        public string GetString(string name) {
            var field = _document.GetField(name);

            return field is not null ? field.GetStringValue() : string.Empty;
        }

        /// <inheritdoc />
        public DateTimeOffset GetDateTimeOffset(string name) {
            var field = _document.GetField(name);

            return field is not null ? DateTools.StringToDate(field.GetStringValue()) : DateTimeOffset.MinValue;
        }

        #endregion
    }
}

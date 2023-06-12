﻿using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Nameless.Text {

    /// <summary>
    /// Default implementation of <see cref="IDataBinder"/>
    /// </summary>
    public sealed class DataBinder : IDataBinder {

        #region Private Static Read-Only Fields

        private static readonly char[] ExpressionPartSeparator = { '.' };
        private static readonly char[] IndexerExpressionStartChars = { '[', '(' };
        private static readonly char[] IndexerExpressionEndChars = { ']', ')' };

        #endregion

        #region Private Static Methods

        private static object? InnerEval(object container, string expression) {
            var expressionParts = expression.Split(ExpressionPartSeparator);

            return InnerEval(container, expressionParts);
        }

        private static object? InnerEval(object container, string[] expressionParts) {
            object? property;
            int index;

            for (property = container, index = 0; (index < expressionParts.Length) && (property != default); index++) {
                var expressionPart = expressionParts[index];
                var indexerExpression = expressionPart.IndexOfAny(IndexerExpressionStartChars) >= 0;

                property = indexerExpression
                    ? GetIndexedPropertyValue(property, expressionPart)
                    : GetPropertyValue(property, expressionPart);
            }

            return property;
        }

        private static object? GetPropertyValue(object container, string expressionPart) {
            var descriptor = TypeDescriptor
                .GetProperties(container)
                .Find(expressionPart, ignoreCase: false);

            if (descriptor == default) {
                throw new ExpressionPropertyNotFoundException(expressionPart);
            }

            return descriptor.GetValue(container);
        }

        private static object? GetIndexedPropertyValue(object container, string expressionPart) {
            var indexExpressionStart = expressionPart.IndexOfAny(IndexerExpressionStartChars);
            var indexExpressionEnd = expressionPart.IndexOfAny(IndexerExpressionEndChars, indexExpressionStart + 1);

            if ((indexExpressionStart < 0) || (indexExpressionEnd < 0) || (indexExpressionEnd == indexExpressionStart + 1)) {
                throw new ArgumentException("Invalid indexer expression.");
            }

            string? propertyName = default;
            if (indexExpressionStart > 0) {
                propertyName = expressionPart[..indexExpressionStart];
            }

            var hasIndex = false;
            object? indexValue = default;
            var indexToken = expressionPart.Substring(
                startIndex: indexExpressionStart + 1,
                length: indexExpressionEnd - indexExpressionStart - 1
            ).Trim();

            if (indexToken.Length != 0) {
                const string doubleQuote = "\"";
                const string singleQuote = "\'";

                if (!(indexToken.StartsWith(singleQuote) && indexToken.EndsWith(singleQuote)) && !(indexToken.StartsWith(doubleQuote) && indexToken.EndsWith(doubleQuote))) {
                    hasIndex = int.TryParse(indexToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intIndex);

                    if (hasIndex) { indexValue = intIndex; } else { indexValue = indexToken; }
                } else { indexValue = indexToken[1..^1]; }
            }

            if (indexValue == default) {
                throw new ArgumentException("Invalid expression.");
            }

            var collection = !string.IsNullOrWhiteSpace(propertyName)
                ? GetPropertyValue(container, propertyName)
                : container;

            if (collection == default) { return default; }
            if (collection is IList list && hasIndex) {
                return list[(int)indexValue];
            }

            var collectionProperty = collection.GetType()
                .GetTypeInfo()
                .GetProperty(
                    name: "Item",
                    returnType: default,
                    types: new[] { indexValue.GetType() },
                    modifiers: default
                );

            if (collectionProperty == default) {
                throw new IndexerAccessorNotFoundException(collection.GetType().FullName!);
            }

            return collectionProperty.GetValue(collection, new[] { indexValue });
        }

        #endregion

        #region IDataBinder Members

        /// <inheritdoc/>
        public object Eval(object container, string expression, string? format = default) {
            Prevent.Null(container, nameof(container));
            Prevent.NullOrWhiteSpaces(expression, nameof(expression));

            var value = InnerEval(container, expression);

            if (value == default || value == DBNull.Value) { return string.Empty; }

            return !string.IsNullOrWhiteSpace(format)
                ? string.Format(format, value)
                : value;
        }

        #endregion
    }
}

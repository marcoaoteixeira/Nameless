﻿using System;

namespace Nameless.Text {

    /// <summary>
    /// Implementation for string pad.
    /// </summary>
	public sealed class StringPadderFormatter : ICustomFormatter {

        #region Public Properties

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public StringPadderDirection Direction { get; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="StringPadderFormatter"/>.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public StringPadderFormatter(StringPadderDirection direction = StringPadderDirection.Right) {
            Direction = direction;
        }

        #endregion Public Constructors

        #region ICustomFormatter Members

        /// <inheritdoc />
        public string Format(string format, object arg, IFormatProvider formatProvider) {
            if (arg == null) { return string.Empty; }

            var paddingChar = format[0];
            var width = int.Parse(format.Substring(1));

            return Direction == StringPadderDirection.Left
                ? arg.ToString().PadLeft(width, paddingChar)
                : arg.ToString().PadRight(width, paddingChar);
        }

        #endregion ICustomFormatter Members
    }
}
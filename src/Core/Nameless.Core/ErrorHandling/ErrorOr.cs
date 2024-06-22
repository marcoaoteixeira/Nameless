#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.ErrorHandling {
    public interface IErrorOr {
        #region Properties

        Error? Error { get; }
        object? Value { get; }
        bool IsError { get; }
        string? Type { get; }

        #endregion
    }

    public static class ErrorOrExtension {
        #region Public Static Methods

        public static TValue? GetValue<TValue>(this IErrorOr self)
            => self.Value is not null &&
               typeof(TValue).IsAssignableTo(self.Value.GetType())
                   ? (TValue)self.Value
                   : default;

        #endregion
    }

    public record ErrorOr : IErrorOr {
        #region Private Read-Only Fields

        private readonly object? _value;

        #endregion

        #region Private Constructors

        private ErrorOr(Error? error, object? value, string? type) {
            if (error is null && value is null) {
                throw new ArgumentException($"Parameter {nameof(error)} or {nameof(value)} should be defined.");
            }

            Error = error;
            _value = value;
            Type = type;
        }

        #endregion

        #region Public Static Methods

        public static IErrorOr Success(object value)
            => new ErrorOr(error: null, value, type: null);

        public static IErrorOr Failure(Error error, string? type = null)
            => new ErrorOr(error: error, value: null, type: type);

        public static IErrorOr Failure(string code, string[] problems, string? type = null)
            => new ErrorOr(error: new Error(code, problems), value: null, type: type);

        #endregion

        #region IErrorOr Members

        public Error? Error { get; }

        public object? Value {
            get {
                if (IsError) {
                    throw new InvalidOperationException($"{nameof(Value)} property cannot be accessed when error have been recorded. Check {nameof(IsError)} before accessing {nameof(Value)}.");
                }

                return _value;
            }
        }

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: false, nameof(Value))]
        [MemberNotNullWhen(returnValue: true, nameof(Type))]
#endif
        public bool IsError => Error is not null;

        public string? Type { get; } 

        #endregion
    }
}

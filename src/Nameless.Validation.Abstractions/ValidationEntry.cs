﻿namespace Nameless.Validation.Abstractions;

public sealed record ValidationEntry {
    public string Code { get; }

    public string Message { get; }

    public ValidationEntry(string code, string message) {
        if (string.IsNullOrWhiteSpace(code)) {
            throw new ArgumentException(Constants.Exceptions.StringNullOrWhiteSpace, nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message)) {
            throw new ArgumentException(Constants.Exceptions.StringNullOrWhiteSpace, nameof(message));
        }

        Code = code;
        Message = message;
    }
}
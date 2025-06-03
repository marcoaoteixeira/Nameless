﻿namespace Nameless.Mediator.Requests;

/// <summary>
///     Represents an async continuation for the next task to execute in the pipeline
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
/// <returns>Awaitable task returning a <typeparamref name="TResponse" /></returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);
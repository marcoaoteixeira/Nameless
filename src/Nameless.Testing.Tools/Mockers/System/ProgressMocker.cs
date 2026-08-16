using Moq;

namespace Nameless.Testing.Tools.Mockers.System;

public class ProgressMocker<T> : Mocker<IProgress<T>> {
    public ProgressMocker<T> WithReport(Action<T> callback) {
        MockInstance
            .Setup(mock => mock.Report(It.IsAny<T>()))
            .Callback(callback);

        return this;
    }
}

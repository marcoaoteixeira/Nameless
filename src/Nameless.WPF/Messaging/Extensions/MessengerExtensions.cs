using Nameless.WPF.Helpers;
using Nameless.WPF.Messaging.Impl;

namespace Nameless.WPF.Messaging;

public static class MessengerExtensions {
    extension(IMessenger self) {
        public Task PublishInformationAsync<TMessage>(string content, string? title = null, object? metadata = null)
            where TMessage : Message, new() {
            return self.InnerPublishAsync<TMessage>(MessageType.Information, content, title, metadata);
        }

        public Task PublishSuccessAsync<TMessage>(string content, string? title = null, object? metadata = null)
            where TMessage : Message, new() {
            return self.InnerPublishAsync<TMessage>(MessageType.Success, content, title, metadata);
        }

        public Task PublishWarningAsync<TMessage>(string content, string? title = null, object? metadata = null)
            where TMessage : Message, new() {
            return self.InnerPublishAsync<TMessage>(MessageType.Warning, content, title, metadata);
        }

        public Task PublishFailureAsync<TMessage>(string content, string? title = null, object? metadata = null)
            where TMessage : Message, new() {
            return self.InnerPublishAsync<TMessage>(MessageType.Failure, content, title, metadata);
        }

        public Task SnackBarInformationAsync(string content, string? title = null, object? metadata = null) {
            return self.InnerPublishAsync<SnackBarMessage>(MessageType.Information, content, title, metadata);
        }

        public Task SnackBarSuccessAsync(string content, string? title = null, object? metadata = null) {
            return self.InnerPublishAsync<SnackBarMessage>(MessageType.Success, content, title, metadata);
        }

        public Task SnackBarWarningAsync(string content, string? title = null, object? metadata = null) {
            return self.InnerPublishAsync<SnackBarMessage>(MessageType.Warning, content, title, metadata);
        }

        public Task SnackBarFailureAsync(string content, string? title = null, object? metadata = null) {
            return self.InnerPublishAsync<SnackBarMessage>(MessageType.Failure, content, title, metadata);
        }

        private Task InnerPublishAsync<TMessage>(MessageType type, string content, string? title = null, object? metadata = null)
            where TMessage : Message, new() {
            
            var evt = new TMessage {
                Content = content,
                Title = title ?? typeof(TMessage).Name,
                Metadata = ObjectHelper.Transform(metadata),
                Type = type
            };

            return self.PublishAsync(evt);
        }
    }
}

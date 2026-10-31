using Nameless.Helpers;
using Nameless.Windows.Events;

namespace Nameless.Windows.Notification;

/// <summary>
///     Event Publisher Extension Methods
/// </summary>
public static class EventPublisherExtensions {
    /// <param name="self">
    ///     The current <see cref="IEventPublisher"/> instance.
    /// </param>
    extension(IEventPublisher self) {
        /// <summary>
        ///     Publishes an information event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the notification event.
        /// </typeparam>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task PublishInformationAsync<TNotificationEvent>(string message, string? title = null, object? metadata = null)
            where TNotificationEvent : NotificationEvent, new() {
            return self.InnerPublishAsync<TNotificationEvent>(NotificationType.Information, message, title, metadata);
        }

        /// <summary>
        ///     Publishes a success event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the notification event.
        /// </typeparam>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task PublishSuccessAsync<TNotificationEvent>(string message, string? title = null, object? metadata = null)
            where TNotificationEvent : NotificationEvent, new() {
            return self.InnerPublishAsync<TNotificationEvent>(NotificationType.Success, message, title, metadata);
        }

        /// <summary>
        ///     Publishes a warning event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the notification event.
        /// </typeparam>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task PublishWarningAsync<TNotificationEvent>(string message, string? title = null, object? metadata = null)
            where TNotificationEvent : NotificationEvent, new() {
            return self.InnerPublishAsync<TNotificationEvent>(NotificationType.Warning, message, title, metadata);
        }

        /// <summary>
        ///     Publishes a failure event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the notification event.
        /// </typeparam>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task PublishFailureAsync<TNotificationEvent>(string message, string? title = null, object? metadata = null)
            where TNotificationEvent : NotificationEvent, new() {
            return self.InnerPublishAsync<TNotificationEvent>(NotificationType.Failure, message, title, metadata);
        }

        private Task InnerPublishAsync<TNotificationEvent>(NotificationType type, string message, string? title = null, object? metadata = null)
            where TNotificationEvent : NotificationEvent, new() {
            
            var evt = new TNotificationEvent {
                Title = title ?? typeof(TNotificationEvent).Name,
                Message = message,
                Metadata = ObjectHelper.Transform(metadata),
                Type = type
            };

            return self.PublishAsync(evt);
        }
    }
}

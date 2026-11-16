using Nameless.Contracts;
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
        ///     Publishes a notification event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the notification event.
        /// </typeparam>
        /// <param name="action">
        ///     The delegate that configures the notification event object.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task PublishAsync<TNotificationEvent>(Action<TNotificationEvent> action) where TNotificationEvent : NotificationEvent, new() {
            return self.PublishAsync(
                ActionHelper.FromDelegate(action)
            );
        }

        /// <summary>
        ///     Publishes an information notification event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the event.
        /// </typeparam>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="metadata">
        ///     Any notification event metadata.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task InformationAsync<TNotificationEvent>(string message, string? title = null, object? metadata = null) where TNotificationEvent : NotificationEvent, new() {
            return self.InnerPublishByTypeAsync<TNotificationEvent>(NotificationType.Information, message, title, metadata);
        }

        /// <summary>
        ///     Publishes a success notification event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the event.
        /// </typeparam>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="metadata">
        ///     Any notification event metadata.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task SuccessAsync<TNotificationEvent>(string message, string? title = null, object? metadata = null) where TNotificationEvent : NotificationEvent, new() {
            return self.InnerPublishByTypeAsync<TNotificationEvent>(NotificationType.Success, message, title, metadata);
        }

        /// <summary>
        ///     Publishes a warning notification event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the event.
        /// </typeparam>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="metadata">
        ///     Any notification event metadata.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task WarningAsync<TNotificationEvent>(string message, string? title = null, object? metadata = null) where TNotificationEvent : NotificationEvent, new() {
            return self.InnerPublishByTypeAsync<TNotificationEvent>(NotificationType.Warning, message, title, metadata);
        }

        /// <summary>
        ///     Publishes a failure notification event.
        /// </summary>
        /// <typeparam name="TNotificationEvent">
        ///     Type of the event.
        /// </typeparam>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="title">
        ///     The title.
        /// </param>
        /// <param name="metadata">
        ///     Any notification event metadata.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous methods
        ///     execution.
        /// </returns>
        public Task FailureAsync<TNotificationEvent>(string message, string? title = null, object? metadata = null) where TNotificationEvent : NotificationEvent, new() {
            return self.InnerPublishByTypeAsync<TNotificationEvent>(NotificationType.Failure, message, title, metadata);
        }

        private Task InnerPublishByTypeAsync<TNotificationEvent>(NotificationType type, string message, string? title = null, object? metadata = null) where TNotificationEvent : NotificationEvent, new() {
            return self.PublishAsync<TNotificationEvent>(evt => {
                evt.Type = type;
                evt.Title = string.IsNullOrWhiteSpace(title) ? typeof(TNotificationEvent).GetPrettyName() : title;
                evt.Message = Throws.When.NullOrWhiteSpace(message);
                evt.Metadata = ObjectHelper.Transform(metadata);
            });
        }
    }
}

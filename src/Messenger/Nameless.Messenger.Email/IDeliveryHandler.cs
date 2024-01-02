﻿using MimeKit;

namespace Nameless.Messenger.Email {
    public interface IDeliveryHandler {
        #region Properties

        DeliveryMode Mode { get; }

        #endregion

        #region Methods

        Task HandleAsync(MimeMessage message, CancellationToken cancellationToken = default);

        #endregion
    }
}
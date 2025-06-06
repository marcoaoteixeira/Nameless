using MimeKit;

namespace Nameless.Mailing.MailKit.Internals;

/// <summary>
/// Helper class for managing email addresses in MimeKit.
/// </summary>
internal static class AddressHelper {
    /// <summary>
    /// Sets the recipients of an InternetAddressList from a collection of email addresses.
    /// </summary>
    /// <param name="addressList">The address list.</param>
    /// <param name="addresses">The address to add to the list.</param>
    internal static void SetRecipients(InternetAddressList addressList, IEnumerable<string> addresses) {
        foreach (var address in addresses) {
            if (string.IsNullOrWhiteSpace(address)) {
                continue;
            }

            if (InternetAddress.TryParse(address, out var recipient)) {
                addressList.Add(recipient);
            }
        }
    }
}
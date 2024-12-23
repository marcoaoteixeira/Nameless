using MimeKit;

namespace Nameless.Mailing.MailKit.Internals;

internal static class AddressHelper {
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
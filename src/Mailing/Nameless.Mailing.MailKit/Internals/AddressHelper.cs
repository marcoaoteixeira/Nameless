using MimeKit;

namespace Nameless.Mailing.MailKit.Internals;

internal static class AddressHelper {
    #region Internal Static Methods

    internal static void SetRecipients(InternetAddressList addressList, IEnumerable<string> addresses) {
        foreach (var address in addresses) {
            if (InternetAddress.TryParse(address, out var recipient)) {
                addressList.Add(recipient);
            }
        }
    }

    internal static string[] SplitAddresses(string csv)
        => csv
            .Split(separator: Constants.Separators.COMMA,
                   options: StringSplitOptions.RemoveEmptyEntries);

    #endregion
}
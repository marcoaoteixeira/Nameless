using Nameless.WPF.Messaging;
using Nameless.WPF.Resources;

namespace Nameless.WPF.UseCases.SystemUpdate.Fetch;

public record FetchNewVersionInformationMessage : Message {
    public FetchNewVersionInformationMessage() {
        Title = Strings.FetchNewVersionInformationNotification_Title;
    }
}
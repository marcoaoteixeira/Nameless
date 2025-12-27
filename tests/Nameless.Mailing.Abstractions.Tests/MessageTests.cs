using System.Text;
using Nameless.Testing.Tools.Data;

namespace Nameless.Mailing;

public class MessageTests {
    [Theory]
    [ClassData(typeof(StringNullEmptyWhiteSpaceExceptionInlineData))]
    public void WhenConstructingMessage_WhenSubjectIsNullEmptyOrWhitespaces_ThenThrowsException(string subject,
        Type exceptionType) {
        Assert.Throws(exceptionType, () => new Message(subject, ["Address"], ["Address"], content: "Content"));
    }

    [Theory]
    [ClassData(typeof(ArrayNullEmptyExceptionInlineData<string>))]
    public void WhenConstructingMessage_WhenFromIsNullOrEmpty_ThenThrowsException(string[] from, Type exceptionType) {
        Assert.Throws(exceptionType, () => new Message(subject: "Subject", from, ["Address"], content: "Content"));
    }

    [Theory]
    [ClassData(typeof(ArrayNullEmptyExceptionInlineData<string>))]
    public void WhenConstructingMessage_WhenToIsNullOrEmpty_ThenThrowsException(string[] to, Type exceptionType) {
        Assert.Throws(exceptionType, () => new Message(subject: "Subject", ["Address"], to, content: "Content"));
    }

    [Theory]
    [ClassData(typeof(StringNullEmptyWhiteSpaceExceptionInlineData))]
    public void WhenConstructingMessage_WhenContentIsNullEmptyOrWhitespaces_ThenThrowsException(string content,
        Type exceptionType) {
        Assert.Throws(exceptionType, () => new Message(subject: "Subject", ["Address"], ["Address"], content));
    }

    [Fact]
    public void WhenConstructingMessage_WithValidParameters_ThenReturnsMessageWithConfiguredParameters() {
        // arrange
        const string Subject = nameof(Subject);
        string[] from = [nameof(from)];
        string[] to = [nameof(to)];
        const string Content = nameof(Content);
        string[] cc = [nameof(cc)];
        string[] bcc = [nameof(bcc)];
        var encoding = Encoding.ASCII;
        const string Language = nameof(Language);
        const bool IsBodyHtml = true;
        const Priority PriorityValue = Priority.High;

        // act
        var sut = new Message(Subject, from, to, Content, cc, bcc, encoding, Language, IsBodyHtml, PriorityValue);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(Subject, sut.Subject);
            Assert.Equivalent(from, sut.From);
            Assert.Equivalent(to, sut.To);
            Assert.Equal(Content, sut.Content);
            Assert.Equivalent(cc, sut.Cc);
            Assert.Equivalent(bcc, sut.Bcc);
            Assert.Equal(encoding, sut.Encoding);
            Assert.Equal(Language, sut.Language);
            Assert.Equal(IsBodyHtml, sut.IsBodyHtml);
            Assert.Equal(PriorityValue, sut.Priority);
        });
    }
}
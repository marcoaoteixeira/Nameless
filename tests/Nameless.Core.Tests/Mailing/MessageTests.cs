using System.Text;
using Nameless.Mailing;

namespace Nameless;

public class MessageTests {
    // --- Constructor ---

    [Fact]
    public void Constructor_WithValidArgs_SetsProperties() {
        // act
        var msg = new Message(
            subject: "Hello",
            from: ["sender@test.com"],
            to: ["recipient@test.com"],
            content: "Body text"
        );

        // assert
        Assert.Multiple(() => {
            Assert.Equal("Hello", msg.Subject);
            Assert.Equal(["sender@test.com"], msg.From);
            Assert.Equal(["recipient@test.com"], msg.To);
            Assert.Equal("Body text", msg.Content);
        });
    }

    [Fact]
    public void Constructor_WithOptionalArgs_SetsAllProperties() {
        // act
        var msg = new Message(
            subject: "Subject",
            from: ["from@test.com"],
            to: ["to@test.com"],
            content: "Content",
            cc: ["cc@test.com"],
            bcc: ["bcc@test.com"],
            encoding: Encoding.ASCII,
            language: "en-US",
            isBodyHtml: true,
            priority: Priority.High
        );

        // assert
        Assert.Multiple(() => {
            Assert.Equal(["cc@test.com"], msg.Cc);
            Assert.Equal(["bcc@test.com"], msg.Bcc);
            Assert.Same(Encoding.ASCII, msg.Encoding);
            Assert.Equal("en-US", msg.Language);
            Assert.True(msg.IsBodyHtml);
            Assert.Equal(Priority.High, msg.Priority);
        });
    }

    [Fact]
    public void Constructor_WithNullSubject_Throws() {
        Assert.ThrowsAny<Exception>(() => new Message(
            subject: null!,
            from: ["from@test.com"],
            to: ["to@test.com"],
            content: "Content"
        ));
    }

    [Fact]
    public void Constructor_WithEmptyFrom_Throws() {
        Assert.ThrowsAny<Exception>(() => new Message(
            subject: "Subject",
            from: [],
            to: ["to@test.com"],
            content: "Content"
        ));
    }

    [Fact]
    public void Constructor_WithDefaults_HasNormalPriorityAndEmptyCcBcc() {
        // act
        var msg = new Message(
            subject: "Subject",
            from: ["from@test.com"],
            to: ["to@test.com"],
            content: "Content"
        );

        // assert
        Assert.Multiple(() => {
            Assert.Equal(Priority.Normal, msg.Priority);
            Assert.Empty(msg.Cc);
            Assert.Empty(msg.Bcc);
            Assert.False(msg.IsBodyHtml);
            Assert.Null(msg.Language);
        });
    }
}

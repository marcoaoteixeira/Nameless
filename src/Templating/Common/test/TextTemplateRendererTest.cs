using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Nameless.Text;
using Xunit;

namespace Nameless.Templating.Common.Test {
    public class TextTemplateRendererTest {
        [Fact]
        public async void RenderAsyncTest () {
            // arrange
            var dataBinder = new DataBinder ();
            var interpolator = new Interpolator (dataBinder);
            var rootPath = Path.Combine (typeof (TextTemplateRendererTest).Assembly.GetDirectoryPath (), "Resources", "Templates");
            using var fileProvider = new PhysicalFileProvider (rootPath);
            using var stream = fileProvider.GetFileInfo ("Test.mail").CreateReadStream ();
            var renderer = new TextTemplateRenderer (interpolator);
            var stringBuilder = new StringBuilder ();
            var template = new TextTemplate {
                Name = "Test.mail",
                State = new {
                    Name = "John Doe",
                    Age = 30,
                    Birthday = new DateTime (1990, 1, 1)
                },
                Text = stream.ToText ()
            };
            using var stringWriter = new StringWriter (stringBuilder);
            var expected = @"This is a test
Your name: John Doe
Your age: 30
Your birthday: 1990-01-01
";

            // act
            await renderer.RenderAsync (template, stringWriter);
            var actual = stringBuilder.ToString ();

            // assert
            Assert.Equal (expected, actual);
        }
    }
}
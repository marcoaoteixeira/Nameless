using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Nameless.Text;
using Xunit;

namespace Nameless.Mailing.Common.Test {
    public class BodyTemplateRendererTest {
        [Fact]
        public async void Test1 () {
            // arrange
            var dataBinder = new DataBinder ();
            var interpolator = new Interpolator (dataBinder);
            var fileProvider = new PhysicalFileProvider (typeof (BodyTemplateRendererTest).Assembly.GetDirectoryPath ());
            var renderer = new BodyTemplateRenderer (interpolator, fileProvider);
            var stringBuilder = new StringBuilder ();
            using var stringWriter = new StringWriter (stringBuilder);
            var data = new {
                Name = "John Doe",
                Age = 30,
                Birthday = new DateTime (1990, 1, 1)
            };
            var expected = @"This is a test
Your name: John Doe
Your age: 30
Your birthday: 1990-01-01
";

            // act
            await renderer.RenderAsync (stringWriter, "Test.mail", data);
            var actual = stringBuilder.ToString ();

            // assert
            Assert.Equal (expected, actual);
        }
    }
}
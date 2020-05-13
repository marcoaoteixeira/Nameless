using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nameless.Text;

namespace Nameless.Templating {
    public sealed class PlainTextFileTemplateRenderer : ITemplateRenderer<PlainTextFileTemplate> {
        #region Private Read-Only Fields

        private readonly IInterpolator _interpolator;

        #endregion

        #region Public Constructors

        public PlainTextFileTemplateRenderer (IInterpolator interpolator) {
            Prevent.ParameterNull (interpolator, nameof (interpolator));

            _interpolator = interpolator;
        }

        #endregion

        #region ITemplateRenderer<PlainTextFileTemplate> Members

        public async Task RenderAsync (PlainTextFileTemplate template, TextWriter writer, CancellationToken token = default) {
            Prevent.ParameterNull (template, nameof (template));
            Prevent.ParameterNull (writer, nameof (writer));

            if (string.IsNullOrWhiteSpace (template.Text)) { return; }

            // if state of the template is null
            // just read the file and output
            // its contents
            if (template.State == null) {
                token.ThrowIfCancellationRequested ();
                await writer.WriteAsync (template.Text);
                return;
            }

            using var reader = new StringReader (template.Text);
            var line = string.Empty;
            while ((line = await reader.ReadLineAsync ()) != null) {
                token.ThrowIfCancellationRequested ();

                var render = _interpolator.Interpolate (line, template.State);
                await writer.WriteLineAsync (render);
            }
        }

        #endregion
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Nameless.Text;

namespace Nameless.Mailing {
    public sealed class BodyTemplateRenderer : IBodyTemplateRenderer {

        #region Private Read-Only Fields

        private readonly IInterpolator _interpolator;
        private readonly IFileProvider _fileProvider;
        private readonly string _bodyTemplateDirectoryFolder;

        #endregion

        #region Public Constructors

        public BodyTemplateRenderer (IInterpolator interpolator, IFileProvider fileProvider, MailingSettings settings = null) {
            Prevent.ParameterNull (interpolator, nameof (interpolator));
            Prevent.ParameterNull (fileProvider, nameof (fileProvider));

            _interpolator = interpolator;
            _fileProvider = fileProvider;
            _bodyTemplateDirectoryFolder = (settings ?? new MailingSettings ()).BodyTemplateDirectoryFolder;
        }

        #endregion

        #region IBodyTemplateRenderer Members

        public async Task RenderAsync (TextWriter writer, string templateName, object data, CancellationToken token = default) {
            var file = _fileProvider
                .GetDirectoryContents (_bodyTemplateDirectoryFolder)
                .SingleOrDefault (_ => string.Equals (Path.GetFileName (_.PhysicalPath), templateName, StringComparison.OrdinalIgnoreCase));

            if (file == null || !file.Exists) {
                throw new FileNotFoundException ("E-mail body template file not found.", templateName);
            }

            using var reader = new StreamReader (file.CreateReadStream ());
            var line = string.Empty;
            while ((line = await reader.ReadLineAsync ()) != null) {
                token.ThrowIfCancellationRequested ();

                var render = _interpolator.Interpolate (line, data);
                await writer.WriteLineAsync (render);
            }
        }

        #endregion
    }
}
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Templating {
    public interface ITemplateRenderer<TTemplate> where TTemplate : Template {
        #region Methods

        Task RenderAsync (TTemplate template, TextWriter writer, CancellationToken token = default);

        #endregion
    }
}
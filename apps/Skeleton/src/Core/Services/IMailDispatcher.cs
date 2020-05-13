using System.Threading.Tasks;

namespace Nameless.Skeleton.Web.Services
{
    public interface IEmailService
    {
         #region Methods

         Task SendAsync (string email, string templateName, object data = null);
             
         #endregion
    }
}
using System.Threading.Tasks;

namespace Nameless.WebApplication {
    public interface IMessageService {
        #region Methods

        Task SendAsync (string to, string from, string subject, string body);

        #endregion
    }
}
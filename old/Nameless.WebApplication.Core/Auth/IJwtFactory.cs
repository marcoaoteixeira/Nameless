using System.Threading.Tasks;

namespace Nameless.WebApplication.Auth {
    public interface IJwtFactory {
        #region Methods

        Task<AccessToken> GenerateEncodedToken (string id, string userName);

        #endregion
    }
}
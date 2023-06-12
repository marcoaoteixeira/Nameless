namespace Nameless.ErrorHandling {

    public interface IPolicyProvider {

        #region Methods

        IEnumerable<IPolicy> GetPoliciesFor(Exception ex);

        #endregion
    }
}

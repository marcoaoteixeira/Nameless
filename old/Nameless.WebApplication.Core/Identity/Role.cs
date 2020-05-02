namespace Nameless.WebApplication.Identity {
    public class Role {
        #region Public Properties

        public string ID { get; internal set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        #endregion

        #region Public Constructors

        public Role (string id = null) {
            ID = id;
        }

        #endregion   
    }
}
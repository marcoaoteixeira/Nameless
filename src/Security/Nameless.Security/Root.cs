namespace Nameless.Security {
    /// <summary>
    /// This class was proposed to be an root point for this assembly.
    /// 
    /// *** DO NOT IMPLEMENT ANYTHING HERE ***
    /// 
    /// But, it's allowed to use it as a repository for all constants or
    /// default values that you'll use throughout this project.
    /// </summary>
    public static class Root {
        #region Public Static Inner Classes

        public static class Defaults {
            #region Internal Constants

            internal const string UPPER_CASE_CHARS = "ABCDEFGIJKMNOPQRSTWXYZ";
            internal const string NUMERIC_CHARS = "0123456789";

            internal const string AES_KEY = "2645f92e-244f-434e-a895-e5efafac8a37";

            internal const string RIJNDAEL_PASS_PHRASE = "29850952b3ef9f90";
            internal const string RIJNDAEL_IV = "9e209040c863f84a";
            internal const string RIJNDAEL_SALT = "c11083b4b0a7743af748c85d343dfee9fbb8b2576c05f3a7f0d632b0926aadfc2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b982408eac03b80adc33dc7d8fbe44b7c7b05d3a2c511166bdb43fcb710b03ba919e79e209040c863f84a31e719795b2577523954739fe5ed3b58a75cff2127075ed1e4ba5cbd251c98e6cd1c23f126a3b81d8d8328abc95387229850952b3ef9f904d1d3ec2e6f20fd420d50e2642992841d8338a314b8ea157c9e18477aaef226ab5206b8b8a996cf5320cb12ca91c7b790fba9f030408efe83ebb83548dc3007bda49670c3c18b9e079b9cfaf51634f563dc8ae3070db2c4a8544305df1b60f007";

            #endregion
        }

        public static class EnvTokens {
            #region Public Constants

            public const string AES_KEY = nameof(AES_KEY);

            public const string RIJNDAEL_PASS_PHRASE = nameof(RIJNDAEL_PASS_PHRASE);

            #endregion
        }

        #endregion
    }
}

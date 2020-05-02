namespace Nameless.Localization.Test {
    public static class Utils {
        public static int DefaultPluralRule (int count) {
            if (count <= 0) { return 0; }
            if (count == 1) { return 1; }

            return 2; // count > 1
        }
    }
}
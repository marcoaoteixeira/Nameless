namespace Nameless.Data {
    public enum DbTransactionState : int {
        None,

        Committed,

        Rolledback
    }
}
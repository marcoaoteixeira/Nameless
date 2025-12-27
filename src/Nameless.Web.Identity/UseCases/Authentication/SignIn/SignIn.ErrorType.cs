namespace Nameless.Web.Identity.UseCases.Authentication.SignIn;

public enum SignInErrorType {
    None,

    Invalid,

    LockedOut,

    NotAllowed
}
namespace Nameless.Microservices.Application.Identity;

public abstract class User {
    public virtual string Username { get; set; } = string.Empty;
    public virtual string Email { get; set; } = string.Empty;
    public virtual string FirstName { get; set; } = string.Empty;
    public virtual string LastName { get; set; } = string.Empty;
}

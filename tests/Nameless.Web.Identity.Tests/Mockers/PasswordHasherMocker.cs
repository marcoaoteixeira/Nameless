using Microsoft.AspNetCore.Identity;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Web.Identity.Mockers;

public class PasswordHasherMocker : MockerBase<IPasswordHasher<User>>;
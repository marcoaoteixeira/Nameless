using Microsoft.AspNetCore.Identity;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Web.Identity.Mockers;

public class UserClaimsPrincipalFactoryMocker : MockerBase<IUserClaimsPrincipalFactory<User>>;
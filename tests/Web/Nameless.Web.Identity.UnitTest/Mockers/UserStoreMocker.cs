using Microsoft.AspNetCore.Identity;
using Nameless.Mockers;

namespace Nameless.Web.Identity.Mockers;

public class UserStoreMocker : MockerBase<IUserStore<User>>;

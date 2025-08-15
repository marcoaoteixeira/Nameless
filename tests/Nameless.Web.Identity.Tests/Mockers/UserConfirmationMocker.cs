using Microsoft.AspNetCore.Identity;
using Nameless.Testing.Tools.Mockers;
using Nameless.Web.Identity.Entities;

namespace Nameless.Web.Identity.Mockers;

public class UserConfirmationMocker : Mocker<IUserConfirmation<User>>;
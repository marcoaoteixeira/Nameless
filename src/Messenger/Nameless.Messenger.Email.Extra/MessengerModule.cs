using Autofac;
using Nameless.Autofac;

namespace Nameless.Messenger.Email {

	public sealed class MessengerModule : ModuleBase {

		#region Protected Override Methods

		protected override void Load(ContainerBuilder builder) {
			builder
				.RegisterType<MessengerService>()
				.As<IMessengerService>()
				.SetLifetimeScope(LifetimeScopeType.PerScope);

			base.Load(builder);
		}

		#endregion
	}
}

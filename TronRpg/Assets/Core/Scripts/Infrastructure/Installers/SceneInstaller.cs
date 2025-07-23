using Core.Scripts.Character.Enemy;
using Core.Scripts.Logic;
using VContainer;
using VContainer.Unity;

namespace Core.Scripts.Infrastructure.Installers
{
    public class SceneInstaller : LifetimeScope
    {
        public SaveTrigger SaveTrigger;
        public Follow Follow;
        public Attack Attack;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(SaveTrigger);
            builder.RegisterComponent(Follow);
            builder.RegisterComponent(Attack);
        }
    }
}
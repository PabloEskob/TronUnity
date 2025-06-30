using Core.Scripts.Logic;
using VContainer;
using VContainer.Unity;

namespace Core.Scripts.Infrastructure.Installers
{
    public class SceneInstaller : LifetimeScope
    {
        public SaveTrigger SaveTrigger;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(SaveTrigger);
        }
    }
}
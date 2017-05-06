using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CurieBot.Responsders;
using MargieBot;

namespace CurieBot
{
    public class ResponderInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IResponder>()
                .ImplementedBy<HelloResponder>()
                .LifestyleSingleton()
                .NamedAutomatically("HelloResponder"));
        }
    }
}
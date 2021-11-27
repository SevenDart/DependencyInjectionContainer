using System.Threading.Tasks;

namespace DependencyInjectionContainerLibrary.Interfaces
{
    public interface IDependencyProvider
    {
        TDependency Resolve<TDependency>();
    }
}
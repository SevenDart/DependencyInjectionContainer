using System;
using System.Collections.Generic;

namespace DependencyInjectionContainerLibrary.Interfaces
{
    public interface IDependencyConfiguration
    {
        Dictionary<Type, List<Type>> TransientTypes { get; }
        Dictionary<Type, List<Type>> SingletonTypes { get; }
        
        void AddSingleton<TDependency, TImplementation>() where TImplementation: TDependency;
        void AddTransient<TDependency, TImplementation>() where TImplementation: TDependency;
    }
}
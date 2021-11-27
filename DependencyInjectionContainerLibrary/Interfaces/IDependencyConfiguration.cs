using System;
using System.Collections.Generic;

namespace DependencyInjectionContainerLibrary.Interfaces
{
    public interface IDependencyConfiguration
    {
        Dictionary<Type, List<Type>> TransientTypes { get; }
        Dictionary<Type, List<Type>> SingletonTypes { get; }
        public Dictionary<Type, Dictionary<Enum, Type>> SingletonNamedTypes { get; }
        public Dictionary<Type, Dictionary<Enum, Type>> TransientNamedTypes { get; }
        
        void AddSingleton<TDependency, TImplementation>() where TImplementation: TDependency;
        void AddSingleton<TDependency, TImplementation>(Enum implementationName) where TImplementation: TDependency;
        void AddSingleton(Type dependencyType, Type implementationType);
        void AddTransient<TDependency, TImplementation>() where TImplementation: TDependency;
        void AddTransient<TDependency, TImplementation>(Enum implementationName) where TImplementation: TDependency;
        void AddTransient(Type dependencyType, Type implementationType);
    }
}
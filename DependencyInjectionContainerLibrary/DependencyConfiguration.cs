using System;
using System.Collections.Generic;
using DependencyInjectionContainerLibrary.Interfaces;

namespace DependencyInjectionContainerLibrary
{
    public class DependencyConfiguration: IDependencyConfiguration
    {
        public Dictionary<Type, List<Type>> TransientTypes { get; } = new();
        public Dictionary<Type, List<Type>> SingletonTypes { get; } = new();

        public void AddSingleton<TDependency, TImplementation>()
        {
            var dependencyType = typeof(TDependency);
            var implementationType = typeof(TImplementation);
            
            if (!SingletonTypes.ContainsKey(dependencyType))
            {
                SingletonTypes.Add(dependencyType, new List<Type>());
            }

            SingletonTypes[dependencyType].Add(implementationType);
        }

        public void AddTransient<TDependency, TImplementation>()
        {
            var dependencyType = typeof(TDependency);
            var implementationType = typeof(TImplementation);
            
            if (!TransientTypes.ContainsKey(dependencyType))
            {
                TransientTypes.Add(dependencyType, new List<Type>());
            }

            TransientTypes[dependencyType].Add(implementationType);
        }
    }
}
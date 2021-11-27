using System;

namespace DependencyInjectionContainerLibrary
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyNameAttribute: Attribute
    {
        public Enum ImplementationName { get; }
        
        public DependencyNameAttribute(object implementationName)
        {
            ImplementationName = implementationName as Enum;
        }
    }
}
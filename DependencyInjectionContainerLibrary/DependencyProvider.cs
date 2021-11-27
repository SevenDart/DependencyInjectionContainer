using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DependencyInjectionContainerLibrary.Interfaces;

namespace DependencyInjectionContainerLibrary
{
    public class DependencyProvider: IDependencyProvider
    {
        private readonly IDependencyConfiguration _configuration;
        

        public DependencyProvider(IDependencyConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public TDependency Resolve<TDependency>()
        {
            return (TDependency) Resolve(new Stack<Type>());
        }

        private object Resolve(Stack<Type> dependencyStack)
        {
            var targetType = dependencyStack.Pop();
            
            foreach (var target in dependencyStack)
            {
                if (target == targetType)
                {
                    return null;
                }
            }
            
            var constructors = targetType.GetConstructors().OrderByDescending(c =>
            {
                int lostArgsCount = 0;
                foreach (var argument in c.GetParameters())
                {
                    if (argument.ParameterType == typeof(IEnumerable<>))
                    {
                        var genericArg = argument.ParameterType.GenericTypeArguments.First();
                        if (!_configuration.SingletonTypes.ContainsKey(genericArg) 
                            || !_configuration.TransientTypes.ContainsKey(genericArg))
                        {
                            lostArgsCount++;
                        }
                    }
                    else if (!_configuration.SingletonTypes.ContainsKey(argument.ParameterType) 
                             || !_configuration.TransientTypes.ContainsKey(argument.ParameterType))
                    {
                        lostArgsCount++;
                    }
                }
                return lostArgsCount;
            });

            foreach (var constructor in constructors)
            {
                var arguments = new List<object>();
                foreach (var argument in constructor.GetParameters())
                {
                    if (argument.ParameterType == typeof(IEnumerable<>))
                    {
                        var genericArg = argument.ParameterType.GenericTypeArguments.First();
                        
                    }
                    else if (!_configuration.SingletonTypes.ContainsKey(argument.ParameterType) 
                             || !_configuration.TransientTypes.ContainsKey(argument.ParameterType))
                    {
                    }
                }
            }


            return null;
        }
    }
}
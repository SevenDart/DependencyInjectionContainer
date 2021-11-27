using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DependencyInjectionContainerLibrary.Interfaces;

namespace DependencyInjectionContainerLibrary
{
    public class DependencyProvider: IDependencyProvider
    {
        private readonly IDependencyConfiguration _configuration;

        private readonly Dictionary<Type, List<object>> _singletons = new();

        public DependencyProvider(IDependencyConfiguration configuration)
        {
            _configuration = configuration;
            foreach (var singletonType in configuration.SingletonTypes)
            {
                _singletons.Add(singletonType.Key, new List<object>());
            }
        }

        private readonly Mutex _mutex = new Mutex();
        
        public TDependency Resolve<TDependency>() where TDependency : class
        {
            var targetType = typeof(TDependency);
            if (_configuration.SingletonTypes.ContainsKey(targetType))
            {
                _mutex.WaitOne();
                if (_singletons[targetType].Count == 0)
                {
                    var stack = new Stack<Type>();
                    stack.Push(_configuration.SingletonTypes[targetType].First());
                    _singletons[targetType].Add(Resolve(stack));
                }
                _mutex.ReleaseMutex();
                return (TDependency) _singletons[targetType].First();
            }

            if (_configuration.TransientTypes.ContainsKey(targetType))
            {
                var stack = new Stack<Type>();
                stack.Push(_configuration.TransientTypes[targetType].First());
                return (TDependency) Resolve(stack);
            }
            
            if (targetType.GenericTypeArguments.Length != 0 
                && targetType == typeof(IEnumerable<>).MakeGenericType(targetType.GenericTypeArguments.First()))
            {
                targetType = targetType.GenericTypeArguments.First();
                if (_configuration.SingletonTypes.ContainsKey(targetType)
                    || _configuration.TransientTypes.ContainsKey(targetType))
                {
                    return (TDependency) GetAllImplementations(targetType);
                }
            }

            return null;
        }

        private object GetAllImplementations(Type targetType)
        {
            var listType = typeof(List<>).MakeGenericType(targetType);
            var list = Activator.CreateInstance(listType);
            
            if (_configuration.SingletonTypes.ContainsKey(targetType))
            {
                _mutex.WaitOne();
                foreach (var implementationType in _configuration.SingletonTypes[targetType])
                {
                    if (_singletons[targetType].FirstOrDefault(s => s.GetType() == implementationType) == null)
                    {
                        var stack = new Stack<Type>();
                        stack.Push(implementationType);
                        _singletons[targetType].Add(Resolve(stack));
                    }
                }
                
                foreach (var singleton in _singletons[targetType])
                {
                    var obj = new[] {singleton};
                    listType.GetMethod("Add")?.Invoke(list, obj);
                }
                _mutex.ReleaseMutex();
            }

            if (_configuration.TransientTypes.ContainsKey(targetType))
            {
                foreach (var type in _configuration.TransientTypes[targetType])
                {
                    var stack = new Stack<Type>();
                    stack.Push(type);
                    var obj = new[] {Resolve(stack)};
                    listType.GetMethod("Add")?.Invoke(list, obj);
                }
            }
            
            return list;
        } 

        private object Resolve(Stack<Type> dependencyStack)
        {
            var targetType = dependencyStack.Peek();
            
            foreach (var target in dependencyStack.Skip(1))
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
                    if (targetType.GenericTypeArguments.Length != 0 
                        && targetType == typeof(IEnumerable<>).MakeGenericType(targetType.GenericTypeArguments.First()))
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

            object result = null;
            foreach (var constructor in constructors)
            {
                try
                {
                    var arguments = new List<object>();
                    foreach (var argument in constructor.GetParameters())
                    {
                        if (argument.ParameterType.IsValueType)
                        {
                            arguments.Add(CreateDefaultValue(argument.ParameterType));
                        }
                        else
                        {
                            arguments.Add(
                                typeof(IDependencyProvider)
                                    .GetMethod("Resolve")?
                                    .MakeGenericMethod(argument.ParameterType)
                                    .Invoke(this, Array.Empty<object>())
                            );
                        }
                    }
                    result = constructor.Invoke(arguments.ToArray());
                }
                catch
                {
                    //Ignore
                }
            }

            return result;
        }
        
        private static object CreateDefaultValue(Type t)
        {
            return t.IsValueType 
                ? Activator.CreateInstance(t) 
                : null;
        }
    }
}
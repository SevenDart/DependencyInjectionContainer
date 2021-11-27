using DependencyInjectionContainerLibrary.Interfaces;
using NUnit.Framework;

namespace DependencyInjectionContainerLibrary.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ResolveAService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IAService testService = dependencyProvider.Resolve<IAService>();

            //Assert
            Assert.NotNull(testService);
        }
        
        [Test]
        public void ResolveBService()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>();
            configuration.AddTransient<IBService, BService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IBService testService = dependencyProvider.Resolve<IBService>();

            //Assert
            Assert.NotNull(testService);
            Assert.NotNull(testService.AService);
        }
        
        [Test]
        public void ResolveTransientAServices()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddTransient<IAService, AService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IAService testServiceOne = dependencyProvider.Resolve<IAService>();
            IAService testServiceTwo = dependencyProvider.Resolve<IAService>();

            //Assert
            Assert.AreNotEqual(testServiceOne, testServiceTwo);
        }
        
        [Test]
        public void ResolveSingletonAServices()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddSingleton<IAService, AService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IAService testServiceOne = dependencyProvider.Resolve<IAService>();
            IAService testServiceTwo = dependencyProvider.Resolve<IAService>();

            //Assert
            Assert.AreEqual(testServiceOne, testServiceTwo);
        }
        
        [Test]
        public void ResolveSingletonAServicesAndTransientBServices()
        {
            //Arrange
            var configuration = new DependencyConfiguration();
            configuration.AddSingleton<IAService, AService>();
            configuration.AddTransient<IBService, BService>();
            var dependencyProvider = new DependencyProvider(configuration);

            //Act
            IBService testServiceOne = dependencyProvider.Resolve<IBService>();
            IBService testServiceTwo = dependencyProvider.Resolve<IBService>();

            //Assert
            Assert.AreNotEqual(testServiceOne, testServiceTwo);
            Assert.AreEqual(testServiceOne.AService, testServiceTwo.AService);
        }
    }
}
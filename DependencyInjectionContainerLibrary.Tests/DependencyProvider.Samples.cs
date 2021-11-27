namespace DependencyInjectionContainerLibrary.Tests
{
    interface IAService
    {
        
    }

    class AService : IAService
    {
        
    }

    class AServiceDuplicate : IAService
    {
        
    }

    interface IBService
    {
        public IAService AService { get; }
    }

    class BService : IBService
    {
        public IAService AService { get; }
        
        public BService(IAService aService)
        {
            AService = aService;
        }
    }
}
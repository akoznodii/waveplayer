using System;
using System.Collections.Generic;
using SimpleInjector;

namespace WavePlayer.Ioc
{
    public class Container : IContainer
    {
        private static readonly Lazy<IContainer> _containerLazy = new Lazy<IContainer>(() => new Container(), true);
        private readonly SimpleInjector.Container _container;

        private Container()
        {
            _container = new SimpleInjector.Container();
        }

        public static IContainer Instance { get { return _containerLazy.Value; } }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register<TService, TImplementation>(Lifestyle.Singleton);
        }

        public void Register<TService>() where TService : class
        {
            _container.Register<TService>(Lifestyle.Singleton);
        }

        public void Register<TService>(Func<TService> instanceCreator) where TService : class
        {
            _container.Register(instanceCreator, Lifestyle.Singleton);
        }

        public void RegisterAll(params Type[] singletonServiceTypes)
        {
            foreach (var singletoneType in singletonServiceTypes)
            {
                _container.Register(singletoneType, singletoneType, Lifestyle.Singleton);
            }
        }

        public void RegisterAll<TService>(params Type[] singletonServiceTypes) where TService : class
        {
            _container.RegisterAll<TService>(singletonServiceTypes);
        }

        public TService GetInstance<TService>() where TService : class
        {
            return _container.GetInstance<TService>();
        }

        public void RegisterInstance(Type serviceType, object instance)
        {
            _container.RegisterSingle(serviceType, instance);
        }

        public IEnumerable<TService> GetAllInstances<TService>() where TService : class
        {
           return _container.GetAllInstances<TService>();
        }
    }
}

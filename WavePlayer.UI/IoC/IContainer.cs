﻿using System;
using System.Collections.Generic;

namespace WavePlayer.UI.Ioc
{
    public interface IContainer
    {
        void RegisterInstance(Type serviceType, object instance);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As design")]
        void Register<TService, TImplementation>() where TService : class where TImplementation : class, TService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As design")]
        void Register<TService>() where TService : class;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "As design")]
        void RegisterAll<TService>(params Type[] singletonServiceTypes) where TService : class;

        TService GetInstance<TService>() where TService : class;

        IEnumerable<TService> GetAllInstances<TService>() where TService : class;
    }
}

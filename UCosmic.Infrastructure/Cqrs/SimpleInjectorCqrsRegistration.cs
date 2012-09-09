﻿using System.Reflection;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace UCosmic.Cqrs
{
    public static class SimpleInjectorCqrsRegistration
    {
        public static void RegisterQueryProcessor(this Container container, params Assembly[] assemblies)
        {
            container.RegisterSingle<SimpleInjectorQueryProcessor>();
            container.Register<IProcessQueries>(container.GetInstance<SimpleInjectorQueryProcessor>);
            container.RegisterManyForOpenGeneric(typeof(IHandleQueries<,>), assemblies);
        }

        public static void RegisterEventProcessor(this Container container, params Assembly[] assemblies)
        {
            container.RegisterSingle<SimpleInjectorSynchronousEventProcessor>();
            container.Register<IProcessEvents>(container.GetInstance<SimpleInjectorSynchronousEventProcessor>);
            container.RegisterManyForOpenGeneric(typeof(IHandleEvents<>),
                (type, implementations) =>
                    {
                        if (implementations.Length == 1)
                            container.Register(type, implementations[0]);
                        else if (implementations.Length > 1)
                            container.RegisterAll(type, implementations);
                    },
                assemblies);
        }
    }
}

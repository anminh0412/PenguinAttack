namespace Services
{
    using System;
    using System.Collections.Generic;

    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            services.TryAdd(type, service);
        }

        public static void Unregister<T>()
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
                services.Remove(type);
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            throw new Exception($"Service {type.Name} is not registered!");
        }
    }

}
using System;
using System.Collections.Concurrent;

namespace Core.Services
{
    public static class ServiceLocator
    {
        private static readonly ConcurrentDictionary<Type, object> _services = new();

        public static void Register<T>(T instance) where T : class
        {
            if (!_services.TryAdd(typeof(T), instance))
                throw new InvalidOperationException($"Service {typeof(T).Name} already registered.");
        }

        public static void Unregister<T>() where T : class =>
            _services.TryRemove(typeof(T), out _);

        public static T Get<T>() where T : class =>
            _services.TryGetValue(typeof(T), out var svc)
                ? (T)svc
                : throw new InvalidOperationException($"Service {typeof(T).Name} not found.");

        public static bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var s)) { service = (T)s; return true; }
            service = null; return false;
        }

        public static void Reset() => _services.Clear(); // Unit‑tests only
    }
}
using System;
using System.Collections.Generic;
using RMC.Core.Debug;

namespace RMC.Mingletons
{
    public interface ISingletonLookup : IDisposable
    {
        //  Properties ----------------------------------------
        
        //  Methods -------------------------------------------
        void AddSingleton<T>(T instance, string key = "") where T : class;
        bool HasSingleton<T>(string key = "") where T : class;
        T GetSingleton<T>(string key = "") where T : class;
        void RemoveSingleton<T>(string key = "") where T : class;
    }
    
    public class SingletonLookup : ISingletonLookup
    {
        private Dictionary<Type, Dictionary<string, object>> _singletons = new Dictionary<Type, Dictionary<string, object>>();
        private ILogger _logger;

        public SingletonLookup(bool isLoggerEnabled)
        {
            _logger = new Logger(isLoggerEnabled) { Prefix = "[SingletonLookup]" };
        }

        public void AddSingleton<T>(T instance, string key = "") where T : class
        {
            var type = typeof(T);
            if (!_singletons.ContainsKey(type))
            {
                _singletons[type] = new Dictionary<string, object>();
            }

            if (_singletons[type].ContainsKey(key))
            {
                _logger.PrintErr($"AddSingleton() Type = '{type.Name}', Key = '{key}'.");
            }
            else
            {
                _singletons[type][key] = instance;
                _logger.Print($"AddSingleton() Type = '{type.Name}', Key = '{key}'.");
            }
        }

        public bool HasSingleton<T>(string key = "") where T : class
        {
            return GetSingleton<T>(key) != null;
        }

        public T GetSingleton<T>(string key = "") where T : class
        {
            var type = typeof(T);
            if (_singletons.TryGetValue(type, out var instances) &&
                instances.TryGetValue(key, out var instance))
            {
                return (T)instance;
            }

            return default(T);
        }

        public void RemoveSingleton<T>(string key = "") where T : class
        {
            var type = typeof(T);
            if (_singletons.ContainsKey(type) && _singletons[type].ContainsKey(key))
            {
                _singletons[type].Remove(key);
                _logger.Print($"RemoveSingleton() Type = '{type.Name}', Key = '{key}'.");
            }
            else
            {
                _logger.PrintErr($"RemoveSingleton() Type = '{type.Name}', Key = '{key}'.");
            }
        }

        public void Dispose()
        {
            _singletons.Clear();
            _logger = null;
        }
    }
}

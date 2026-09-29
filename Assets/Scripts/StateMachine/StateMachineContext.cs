using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zombies.StateMachine
{
    public sealed class StateMachineContext
    {
        private readonly Dictionary<Type, object> services = new();
        private readonly Dictionary<string, object> values = new();

        public GameObject Owner;
        public float DeltaTime;
        public float ElapsedTime;

        public void SetService<T>(T service) where T : class
        {
            if (service == null) services.Remove(typeof(T));
            else services[typeof(T)] = service;
        }

        public bool TryGetService<T>(out T service) where T : class
        {
            if (services.TryGetValue(typeof(T), out var value) && value is T typed)
            {
                service = typed;
                return true;
            }
            service = null;
            return false;
        }

        public T GetService<T>() where T : class => TryGetService(out T service) ? service : null;

        public void SetValue<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Context key cannot be empty.", nameof(key));
            values[key] = value;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (values.TryGetValue(key, out var raw) && raw is T typed)
            {
                value = typed;
                return true;
            }
            value = default;
            return false;
        }

        public bool RemoveValue(string key) => values.Remove(key);
    }
}

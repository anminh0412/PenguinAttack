namespace SimpleSignalBus
{
    using System;
    using System.Collections.Generic;

    public static class SignalBus
    {
        private static readonly Dictionary<Type, Delegate> EventTable = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (EventTable.TryGetValue(type, out var existing))
            {
                EventTable[type] = Delegate.Combine(existing, callback);
            }
            else
            {
                EventTable[type] = callback;
            }
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);

            if (!EventTable.TryGetValue(type, out var existing)) return;
            var current = Delegate.Remove(existing, callback);
            if (current == null)
                EventTable.Remove(type);
            else
                EventTable[type] = current;
        }

        public static void Fire<T>(T signal)
        {
            var type = typeof(T);
            if (EventTable.TryGetValue(type, out var callback))
            {
                ((Action<T>)callback)?.Invoke(signal);
            }
        }
    }
}
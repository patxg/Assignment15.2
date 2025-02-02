using System;
using System.Collections.Generic;

namespace Helpers
{
    public sealed class EventManager
    {
        private Dictionary<Type, Delegate> eventDictionary = new Dictionary<Type, Delegate>();

        public void Subscribe<T>(Action<T> listener) where T : class
        {
            if (eventDictionary.TryGetValue(typeof(T), out var existingEvent))
            {
                eventDictionary[typeof(T)] = (Action<T>) existingEvent + listener;
            }
            else
            {
                eventDictionary[typeof(T)] = listener;
            }
        }

        public void Unsubscribe<T>(Action<T> listener) where T : class
        {
            if (eventDictionary.TryGetValue(typeof(T), out var existingEvent))
            {
                eventDictionary[typeof(T)] = (Action<T>) existingEvent - listener;
            }
        }

        public void TriggerEvent<T>(T eventData) where T : class
        {
            if (eventDictionary.TryGetValue(typeof(T), out var existingEvent))
            {
                ((Action<T>) existingEvent)?.Invoke(eventData);
            }
        }
    }
}
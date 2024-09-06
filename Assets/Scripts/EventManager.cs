using System;
using System.Collections.Generic;

public static class EventManager
{
    private static Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

    public static void StartListening(string eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            eventDictionary[eventName] = thisEvent + listener;
        }
        else
        {
            eventDictionary.Add(eventName, listener);
        }
    }

    public static void StopListening(string eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            eventDictionary[eventName] = thisEvent - listener;
        }
    }

    public static void TriggerEvent(string eventName, object eventParam)
    {
        if (eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent.Invoke(eventParam);
        }
    }
}

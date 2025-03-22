using System.Collections.Generic;
using DragonBones;
using System;

public class MonoGameEventDispatcher : IEventDispatcher<EventObject>
{
    private Dictionary<string, List<Action<EventObject>>> _listeners;

    public MonoGameEventDispatcher()
    {
        _listeners = new Dictionary<string, List<Action<EventObject>>>();
    }

    public void DispatchDBEvent(string type, EventObject eventObject)
    {
        if (_listeners.ContainsKey(type))
        {
            foreach (var listener in _listeners[type])
            {
                listener(eventObject);
            }
        }

        switch (type)
        {
            case EventObject.START:
                // Animation started
                break;
            
            case EventObject.LOOP_COMPLETE:
                // Animation loop completed
                break;
            
            case EventObject.COMPLETE:
                // Animation completed
                break;
            
            case EventObject.FRAME_EVENT:
                // Custom frame event
                string eventName = eventObject.name;
                break;
        }
    }

    public bool HasDBEventListener(string type)
    {
        return _listeners.ContainsKey(type) && _listeners[type].Count > 0;
    }

    public void AddDBEventListener(string type, Action<EventObject> listener)
    {
        if (!_listeners.ContainsKey(type))
        {
            _listeners[type] = new List<Action<EventObject>>();
        }
        
        if (!_listeners[type].Contains(listener))
        {
            _listeners[type].Add(listener);
        }
    }

    public void RemoveDBEventListener(string type, Action<EventObject> listener)
    {
        if (_listeners.ContainsKey(type))
        {
            _listeners[type].Remove(listener);
        }
    }

    public void AddDBEventListener(string type, ListenerDelegate<EventObject> listener)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveDBEventListener(string type, ListenerDelegate<EventObject> listener)
    {
        throw new System.NotImplementedException();
    }
}
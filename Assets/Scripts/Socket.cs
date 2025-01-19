using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Socket : MonoBehaviour
{
    Dictionary<string, UnityEvent<object>> listeners;
    public void AddListener(string code, UnityAction<object> listener)
    {
        if (listeners == null)
        {
            listeners = new Dictionary<string, UnityEvent<object>>();
        }
        if (!listeners.ContainsKey(code))
        {
            listeners.Add(code, new UnityEvent<object>());
        }
        listeners[code].AddListener(listener);
    }
    public void RemoveListener(string code)
    {
        if (listeners == null)
        {
            listeners = new Dictionary<string, UnityEvent<object>>();
        }
        if (listeners.ContainsKey(code))
        {
            listeners.Remove(code);
        }
    }
    public void Emit(string code, object arg)
    {
        if (listeners == null)
        {
            listeners = new Dictionary<string, UnityEvent<object>>();
        }
        if (listeners.ContainsKey(code))
        {
            listeners[code].Invoke(arg);
        }
    }
}
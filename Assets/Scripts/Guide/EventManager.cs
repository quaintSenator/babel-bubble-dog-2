using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EventManager : BaseManager
{
    public static class EventKeys
    {
        public const string PlayerHitReactableObject = "PlayerHitReactableObject";
        public const string PlayerLeaveReactableObject = "PlayerLeaveReactableObject";
    }

    private static readonly Dictionary<string, Delegate> EventTable = new Dictionary<string, Delegate>(StringComparer.Ordinal);

    private static EventManager instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public static void Register(string eventKey, Action handler)
    {
        if (!ValidateRegister(eventKey, handler))
        {
            return;
        }

        if (EventTable.TryGetValue(eventKey, out Delegate existing))
        {
            if (existing is Action typed)
            {
                EventTable[eventKey] = typed + handler;
                return;
            }

            Debug.LogError($"EventManager.Register failed: event '{eventKey}' already registered with signature {existing.GetType().Name}.");
            return;
        }

        EventTable[eventKey] = handler;
    }

    public static void Register<T>(string eventKey, Action<T> handler)
    {
        if (!ValidateRegister(eventKey, handler))
        {
            return;
        }

        if (EventTable.TryGetValue(eventKey, out Delegate existing))
        {
            if (existing is Action<T> typed)
            {
                EventTable[eventKey] = typed + handler;
                return;
            }

            Debug.LogError($"EventManager.Register failed: event '{eventKey}' already registered with signature {existing.GetType().Name}.");
            return;
        }

        EventTable[eventKey] = handler;
    }

    public static void Unregister(string eventKey, Action handler)
    {
        if (!ValidateUnregister(eventKey, handler))
        {
            return;
        }

        if (!EventTable.TryGetValue(eventKey, out Delegate existing))
        {
            return;
        }

        if (existing is Action typed)
        {
            typed -= handler;
            UpdateEntry(eventKey, typed);
            return;
        }

        Debug.LogError($"EventManager.Unregister failed: event '{eventKey}' registered with signature {existing.GetType().Name}.");
    }

    public static void Unregister<T>(string eventKey, Action<T> handler)
    {
        if (!ValidateUnregister(eventKey, handler))
        {
            return;
        }

        if (!EventTable.TryGetValue(eventKey, out Delegate existing))
        {
            return;
        }

        if (existing is Action<T> typed)
        {
            typed -= handler;
            UpdateEntry(eventKey, typed);
            return;
        }

        Debug.LogError($"EventManager.Unregister failed: event '{eventKey}' registered with signature {existing.GetType().Name}.");
    }

    public static void Dispatch(string eventKey)
    {
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            Debug.LogError("EventManager.Dispatch failed: eventKey is empty.");
            return;
        }

        if (!EventTable.TryGetValue(eventKey, out Delegate existing))
        {
            return;
        }

        if (existing is Action typed)
        {
            typed.Invoke();
            return;
        }

        Debug.LogError($"EventManager.Dispatch failed: event '{eventKey}' registered with signature {existing.GetType().Name}.");
    }

    public static void Dispatch<T>(string eventKey, T arg)
    {
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            Debug.LogError("EventManager.Dispatch failed: eventKey is empty.");
            return;
        }

        if (!EventTable.TryGetValue(eventKey, out Delegate existing))
        {
            return;
        }

        if (existing is Action<T> typed)
        {
            typed.Invoke(arg);
            return;
        }

        Debug.LogError($"EventManager.Dispatch failed: event '{eventKey}' registered with signature {existing.GetType().Name}.");
    }

    public static void register(string eventKey, Action handler) => Register(eventKey, handler);
    public static void register<T>(string eventKey, Action<T> handler) => Register(eventKey, handler);
    public static void unregister(string eventKey, Action handler) => Unregister(eventKey, handler);
    public static void unregister<T>(string eventKey, Action<T> handler) => Unregister(eventKey, handler);
    public static void dispatch(string eventKey) => Dispatch(eventKey);
    public static void dispatch<T>(string eventKey, T arg) => Dispatch(eventKey, arg);

    private static bool ValidateRegister(string eventKey, Delegate handler)
    {
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            Debug.LogError("EventManager.Register failed: eventKey is empty.");
            return false;
        }

        if (handler == null)
        {
            Debug.LogError("EventManager.Register failed: handler is null.");
            return false;
        }

        return true;
    }

    private static bool ValidateUnregister(string eventKey, Delegate handler)
    {
        if (string.IsNullOrWhiteSpace(eventKey))
        {
            Debug.LogError("EventManager.Unregister failed: eventKey is empty.");
            return false;
        }

        if (handler == null)
        {
            Debug.LogError("EventManager.Unregister failed: handler is null.");
            return false;
        }

        return true;
    }

    private static void UpdateEntry(string eventKey, Delegate updated)
    {
        if (updated == null)
        {
            EventTable.Remove(eventKey);
        }
        else
        {
            EventTable[eventKey] = updated;
        }
    }
}

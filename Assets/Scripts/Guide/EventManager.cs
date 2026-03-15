using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum EventKey
{
    PlayerHitReactableObject,
    PlayerLeaveReactableObject,
    GuideWindowNextStep,
}
public class EventManager : BaseManager
{
   

    private static readonly Dictionary<EventKey, Delegate> EventTable = new Dictionary<EventKey, Delegate>();

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

    public static void Register(EventKey eventKey, Action handler)
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

    public static void Register<T>(EventKey eventKey, Action<T> handler)
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

    public static void Unregister(EventKey eventKey, Action handler)
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

    public static void Unregister<T>(EventKey eventKey, Action<T> handler)
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

    public static void Dispatch(EventKey eventKey)
    {
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

    public static void Dispatch<T>(EventKey eventKey, T arg)
    {
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

    public static void register(EventKey eventKey, Action handler) => Register(eventKey, handler);
    public static void register<T>(EventKey eventKey, Action<T> handler) => Register(eventKey, handler);
    public static void unregister(EventKey eventKey, Action handler) => Unregister(eventKey, handler);
    public static void unregister<T>(EventKey eventKey, Action<T> handler) => Unregister(eventKey, handler);
    public static void dispatch(EventKey eventKey) => Dispatch(eventKey);
    public static void dispatch<T>(EventKey eventKey, T arg) => Dispatch(eventKey, arg);

    private static bool ValidateRegister(EventKey eventKey, Delegate handler)
    {
        if (handler == null)
        {
            Debug.LogError("EventManager.Register failed: handler is null.");
            return false;
        }

        return true;
    }

    private static bool ValidateUnregister(EventKey eventKey, Delegate handler)
    {
        if (handler == null)
        {
            Debug.LogError("EventManager.Unregister failed: handler is null.");
            return false;
        }

        return true;
    }

    private static void UpdateEntry(EventKey eventKey, Delegate updated)
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

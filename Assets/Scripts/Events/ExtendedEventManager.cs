using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// MMGameEvents are used throughout the game for general game events (game started, game ended, life lost, etc.)
/// </summary>
public struct GeneralEvent
{
    public string EventName;
    public GeneralEvent(string newName)
    {
        EventName = newName;
    }
    static GeneralEvent e;
    public static void Trigger(string newName)
    {
        e.EventName = newName;
        ExtendedEventManager.TriggerEvent(e);
    }
}

/// <summary>
/// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
/// Events are structs, you can define any kind of events you want. This manager comes with MMGameEvents, which are 
/// basically just made of a string, but you can work with more complex ones if you want.
/// 
/// To trigger a new event, from anywhere, do YOUR_EVENT.Trigger(YOUR_PARAMETERS)
/// So GameEvent.Trigger("Save"); for example will trigger a Save MMGameEvent
/// 
/// you can also call ExtendedEventManager.TriggerEvent(YOUR_EVENT);
/// For example : ExtendedEventManager.TriggerEvent(new GameEvent("GameStart")); will broadcast an GameEvent named GameStart to all listeners.
///
/// To start listening to an event from any class, there are 3 things you must do : 
///
/// 1 - tell that your class implements the MMEventListener interface for that kind of event.
/// For example: public class GUIManager : Singleton<GUIManager>, ExtendedEventListener<GameEvent>
/// You can have more than one of these (one per event type).
///
/// 2 - On Enable and Disable, respectively start and stop listening to the event :
/// void OnEnable()
/// {
/// 	this.ExtendedEventStartListening<GameEvent>();
/// }
/// void OnDisable()
/// {
/// 	this.ExtendedEventStopListening<GameEvent>();
/// }
/// 
/// 3 - Implement the ExtendedEventListener interface for that event. For example :
/// public void OnMMEvent(MMGameEvent gameEvent)
/// {
/// 	if (gameEvent.EventName == "GameOver")
///		{
///			// DO SOMETHING
///		}
/// } 
/// will catch all events of type MMGameEvent emitted from anywhere in the game, and do something if it's named GameOver
/// </summary>
[ExecuteAlways]
public static class ExtendedEventManager
{
    private static Dictionary<Type, List<ExtendedEventListenerBase>> subscribersList;

    static ExtendedEventManager()
    {
        subscribersList = new Dictionary<Type, List<ExtendedEventListenerBase>>();
    }

    /// <summary>
    /// Adds a new subscriber to a certain event.
    /// </summary>
    /// <param name="listener">listener.</param>
    /// <typeparam name="ExtendedEvent">The event type.</typeparam>
    public static void AddListener<ExtendedEvent>(ExtendedEventListener<ExtendedEvent> listener) where ExtendedEvent : struct
    {
        Type eventType = typeof(ExtendedEvent);

        if (!subscribersList.ContainsKey(eventType))
        {
            subscribersList[eventType] = new List<ExtendedEventListenerBase>();
        }

        if (!SubscriptionExists(eventType, listener))
        {
            subscribersList[eventType].Add(listener);
        }
    }

    /// <summary>
    /// Removes a subscriber from a certain event.
    /// </summary>
    /// <param name="listener">listener.</param>
    /// <typeparam name="ExtendedEvent">The event type.</typeparam>
    public static void RemoveListener<ExtendedEvent>(ExtendedEventListener<ExtendedEvent> listener) where ExtendedEvent : struct
    {
        Type eventType = typeof(ExtendedEvent);

        if (!subscribersList.ContainsKey(eventType))
        {
                #if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
                #else
            return;
            #endif
        }

        List<ExtendedEventListenerBase> subscriberList = subscribersList[eventType];

        #if EVENTROUTER_THROWEXCEPTIONS
	        bool listenerFound = false;
        #endif

        for (int i = subscriberList.Count - 1; i >= 0; i--)
        {
            if (subscriberList[i] == listener)
            {
                subscriberList.Remove(subscriberList[i]);
                #if EVENTROUTER_THROWEXCEPTIONS
					listenerFound = true;
                #endif

                if (subscriberList.Count == 0)
                {
                    subscribersList.Remove(eventType);
                }

                return;
            }
        }

        #if EVENTROUTER_THROWEXCEPTIONS
		    if( !listenerFound )
		    {
				throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		    }
        #endif
    }

    /// <summary>
    /// Triggers an event. All instances that are subscribed to it will receive it (and will potentially act on it).
    /// </summary>
    /// <param name="newEvent">The event to trigger.</param>
    /// <typeparam name="ExtendedEvent">The 1st type parameter.</typeparam>
    public static void TriggerEvent<ExtendedEvent>(ExtendedEvent newEvent) where ExtendedEvent : struct
    {
        List<ExtendedEventListenerBase> list;
        if (!subscribersList.TryGetValue(typeof(ExtendedEvent), out list))
        #if EVENTROUTER_REQUIRELISTENER
			throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
            #else
            return;
        #endif

        for (int i = list.Count - 1; i >= 0; i--)
        {
            (list[i] as ExtendedEventListener<ExtendedEvent>).OnExtendedEvent(newEvent);
        }
    }

    /// <summary>
    /// Checks if there are subscribers for a certain type of events
    /// </summary>
    /// <returns><c>true</c>, if exists was subscriptioned, <c>false</c> otherwise.</returns>
    /// <param name="type">Type.</param>
    /// <param name="receiver">Receiver.</param>
    private static bool SubscriptionExists(Type type, ExtendedEventListenerBase receiver)
    {
        List<ExtendedEventListenerBase> receivers;

        if (!subscribersList.TryGetValue(type, out receivers)) return false;

        bool exists = false;

        for (int i = receivers.Count - 1; i >= 0; i--)
        {
            if (receivers[i] == receiver)
            {
                exists = true;
                break;
            }
        }

        return exists;
    }
}

/// <summary>
/// Static class that allows any class to start or stop listening to events
/// </summary>
public static class EventRegister
{
    public delegate void Delegate<T>(T eventType);

    public static void ExtendedEventStartListening<EventType>(this ExtendedEventListener<EventType> caller) where EventType : struct
    {
        ExtendedEventManager.AddListener<EventType>(caller);
    }

    public static void ExtendedEventStopListening<EventType>(this ExtendedEventListener<EventType> caller) where EventType : struct
    {
        ExtendedEventManager.RemoveListener<EventType>(caller);
    }
}

/// <summary>
/// Event listener basic interface
/// </summary>
public interface ExtendedEventListenerBase { };

/// <summary>
/// A public interface you'll need to implement for each type of event you want to listen to.
/// </summary>
public interface ExtendedEventListener<T> : ExtendedEventListenerBase
{
    void OnExtendedEvent(T eventType);
}

public class ExtendedEventListenerWrapper<TOwner, TTarget, TEvent> : ExtendedEventListener<TEvent>, IDisposable
    where TEvent : struct
{
    private Action<TTarget> _callback;

    private TOwner _owner;
    public ExtendedEventListenerWrapper(TOwner owner, Action<TTarget> callback)
    {
        _owner = owner;
        _callback = callback;
        RegisterCallbacks(true);
    }

    public void Dispose()
    {
        RegisterCallbacks(false);
        _callback = null;
    }

    protected virtual TTarget OnEvent(TEvent eventType) => default;
    public void OnExtendedEvent(TEvent eventType)
    {
        var item = OnEvent(eventType);
        _callback?.Invoke(item);
    }

    private void RegisterCallbacks(bool b)
    {
        if (b)
        {
            this.ExtendedEventStartListening<TEvent>();
        }
        else
        {
            this.ExtendedEventStopListening<TEvent>();
        }
    }
}

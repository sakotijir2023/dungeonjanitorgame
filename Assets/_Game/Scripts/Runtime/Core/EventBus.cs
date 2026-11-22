using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonJanitor.Core
{
    /// <summary>
    /// Static, typed event channel. Systems raise and listen to struct events
    /// without holding references to each other.
    /// </summary>
    public static class EventBus<T> where T : struct
    {
        static event Action<T> Handlers;

        static EventBus()
        {
            EventBusReset.Register(() => Handlers = null);
        }

        public static void Subscribe(Action<T> handler) => Handlers += handler;
        public static void Unsubscribe(Action<T> handler) => Handlers -= handler;
        public static void Raise(T evt) => Handlers?.Invoke(evt);
    }

    /// <summary>
    /// Clears every bus when play mode starts, so stale listeners never survive
    /// when domain reload is disabled.
    /// </summary>
    public static class EventBusReset
    {
        static readonly List<Action> ClearActions = new List<Action>();

        internal static void Register(Action clear) => ClearActions.Add(clear);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ClearAll()
        {
            foreach (var clear in ClearActions)
                clear();
        }
    }
}

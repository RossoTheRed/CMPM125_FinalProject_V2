using System;
using System.Collections.Generic;
public class EventBus
{
    private static EventBus _instance;
    public static EventBus Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EventBus();
            return _instance;
        }
    }

    public enum EventName
    {
        Invalid = 0,
        MainMenu,
        GameStart,
        GamePause,
        GameOver,
    }

    // DoMainMenu when moving to main menu
    public event Action OnMainMenu;
    public void DoMainMenu() => OnMainMenu?.Invoke();
    // DoGameState when moving to game start
    public event Action OnGameStart;
    public void DoGameStart() => OnGameStart?.Invoke();
    // DoGamePause when moving to game pause
    public event Action OnGamePause;
    public void DoGamePause() => OnGamePause?.Invoke();
    // DoGamePause when moving to game over
    public event Action OnGameOver;
    public void DoGameOver() => OnGameOver?.Invoke();

    private readonly Dictionary<Action, Delegate> _activeEventListeners = new Dictionary<Action, Delegate>();  
    // note: I am testing a new format for the event bus

    // register an action
    public void Register(Action listener, EventName eventName)
    {
        Action wrapper = () => listener();
        switch (eventName)
        {
            case EventName.MainMenu: OnMainMenu += wrapper; break;
            case EventName.GameStart: OnGameStart += wrapper; break;
            case EventName.GamePause: OnGamePause += wrapper; break;
            case EventName.GameOver: OnGameOver += wrapper; break;
            default: throw new Exception("Failed Register: Given eventName does not exist as a register - " + eventName);
        }
        
        _activeEventListeners[listener] = wrapper; // Overwrites if exists
    }

    // unregister an action
    public void Deregister(Action listener, EventName eventName)
    {
        if (_activeEventListeners.TryGetValue(listener, out Delegate wrapper))
        {
            switch (eventName)
            {
                case EventName.MainMenu: OnMainMenu -= (Action)wrapper; break;
                case EventName.GameStart: OnGameStart -= (Action)wrapper; break;
                case EventName.GamePause: OnGamePause -= (Action)wrapper; break;
                case EventName.GameOver: OnGameOver -= (Action)wrapper; break;
                default: throw new Exception("Failed Deregister: Given eventName does not exist as a Deregister - " + eventName);
            }
            _activeEventListeners.Remove(listener);

        }
    }
}
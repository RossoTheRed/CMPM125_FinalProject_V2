using UnityEngine;
using System.Collections.Generic;

public class TimeKeeperScript : MonoBehaviour
{

    // Basic singleton behavior from: https://docs.google.com/presentation/d/1qHUVop1Fx578YSjZOXzxAt_0pezWGjDxDmCk7rr-8bA/edit?slide=id.g2b35719668b_0_346#slide=id.g2b35719668b_0_346
    // Thanks, professor!
    private static TimeKeeperScript _instance;
    public static TimeKeeperScript Instance { get { return (_instance); } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        { // if there is already a value assigned to the private variable and its not this, destroy this
            Destroy(this.gameObject);
        }
        else
        { // if there is no value assigned to the private variable, assign this as the reference
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public Stopwatch CreateStopwatch(string name, float max = -1, bool isActive = true)
    {
        if (max != -1 && max <= 0)
        {
            Debug.LogError($"Timer maximum can't be less than 0 other than -1!");
            return(null);
        }

        Stopwatch newTimer = new Stopwatch(name, max, isActive);
        return(newTimer);
    }

    public Countdown CreateCountdown(string name, float duration, bool isActive = true)
    {
        if (duration <= 0)
        {
            Debug.LogError($"Timer duration can't be 0!");
            return(null);
        }

        Countdown newTimer = new Countdown(name, duration, isActive);
        return(newTimer);
    }

    public class Timer
    {
        protected string _name;
        protected float _time;
        protected bool _active;

        public Timer(string name, bool isActive = true)
        {

            _name = name;
            _time = 0;
            _active = isActive;
        }

        public string GetName()
        {
            return (_name);
        }

        public float GetTime()
        {
            return (_time);
        }

        public System.TimeSpan GetFormattedTime()
        {
            return (System.TimeSpan.FromSeconds(GetTime()));
        }

        public bool IsActive()
        {
            return (_active);
        }

        public virtual bool IsComplete()
        {
            Debug.LogError("This method should be re-defined by a sub-class");
            return(false);
        }

        public virtual float GetProgress()
        {
            Debug.LogError("This method should be re-defined by a sub-class");
            return (0f);
        }

        public void Pause()
        {
            _active = false;
        }

        public void Start()
        {
            _active = true;

            if (IsComplete())
            {
                Reset();
            }
        }

        public virtual void Count(float deltaTime)
        {
            Debug.LogError("This method must be re-defined in a sub-class!");
        }

        public virtual void Reset()
        {
            Debug.LogError("This method must be re-defined in a sub-class!");
        }
    }

    public class Stopwatch : Timer
    {
        protected float _max;

        public Stopwatch(string name, float max = -1, bool isActive = true) : base(name, isActive)
        {
            _max = max;
        }

        public override void Count(float deltaTime)
        {
            if (IsActive() == true)
            {
                _time += deltaTime;
            }

            if (IsComplete() && IsActive())
            {
                Pause();
            }
        }

        public override void Reset()
        {
            _time = 0;
        }

        public override bool IsComplete()
        {
            if (_max != -1)
            {
                return(_time <= _max);
            }

            return(false);
        }

        public override float GetProgress()
        {
            if (_max > 0) 
            {
                int progress = (int)( (_time/_max) * 100);
                return(progress/100f);
            }

            return(0f);
        }

        public float GetMaximum()
        {
            return(_max);
        }
    }

    public class Countdown : Timer
    {
        protected float _duration;
        public Countdown(string name, float duration, bool isActive = true) : base(name, isActive)
        {
            _duration = duration;
            _time = duration;
        }

        public override void Count(float deltaTime)
        {
            if (IsActive() == true)
            {
                _time -= deltaTime;
            }

            if (IsComplete() && IsActive())
            {
                Pause();
            }
        }

        public override void Reset()
        {
            _time = _duration;
        }

        public override bool IsComplete()
        {
            return(_time <= 0);
        }

        public override float GetProgress()
        {
            int progress = (int)((_time / _duration) * 100); // Multiply by a hundred and cast to int to get rid of annoying decimals
            return (progress / 100f);
        }

        public float GetDuration()
        {
            return(_duration);
        }
    }
}

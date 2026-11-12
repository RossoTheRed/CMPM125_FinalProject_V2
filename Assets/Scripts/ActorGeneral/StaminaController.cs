using UnityEngine;

public class StaminaController : MonoBehaviour
{
    TimeKeeperScript tk;

    [SerializeField] float baseMaxStamina = 100f;
    [SerializeField] float minMaxStamina = 5f;

        [Tooltip("How often max stamina should degrade, in seconds")]
    [SerializeField] float degradationSpeed = 180;

        [Tooltip("How much max stamina is reduced per cycle")]
    [SerializeField] float degradationAmount = 5;

    string DEGRADE_TIMER_NAME = "degradeTimer";
    TimeKeeperScript.Countdown degradeTimer;
    string degradeTimerValue;

    [Tooltip("How fast stamina regenerates, in seconds")]
    [SerializeField] float regenDelay = 5f;

        [Tooltip("How much stamina regenerates per cycle")]
    [SerializeField] float regenAmount = 1f;
    [SerializeField] bool doRegen = true;

    string REGEN_TIMER_NAME = "regenTimer";
    TimeKeeperScript.Countdown regenTimer;

    string regenTimerValue;

    public float currMaxStamina {get; private set;}
    public float currStamina {get; private set;}

    void Start()
    {
        tk = TimeKeeperScript.Instance;

        
        degradeTimer = tk.CreateCountdown(DEGRADE_TIMER_NAME, degradationSpeed, true);
        regenTimer = tk.CreateCountdown(REGEN_TIMER_NAME, regenDelay, false);

        RestoreMaximum(baseMaxStamina);
        Replenish(currMaxStamina);
    }

    void Update()
    {
        UpdateTimers();
        ReadTimers();

        DegradeOverTime();
        RegenOverTime();
    }

    public void ReduceMaximum(float amount)
    {
        currMaxStamina = Mathf.Clamp(currMaxStamina - amount, minMaxStamina, currMaxStamina - amount);
        currStamina = Mathf.Clamp(currStamina, 0, currMaxStamina);
    }

    public void RestoreMaximum(float amount)
    {
        currMaxStamina = Mathf.Clamp(currMaxStamina + amount, currMaxStamina + amount, baseMaxStamina);
    }

    public void Replenish(float amount)
    {
        currStamina = Mathf.Clamp(currStamina + amount, currStamina + amount, currMaxStamina);
    }

    public void Expend(float amount)
    {
        if (HaveStamina(amount))
        {
            currStamina = Mathf.Clamp(currStamina - amount, 0, currStamina - amount);
        }
    }

    public void DelayRegen()
    {
        doRegen = false;
    }

    public void ResumeRegen()
    {
        doRegen = true;
    }

    void DegradeOverTime()
    {
        if (degradeTimer.IsComplete())
        {
            ReduceMaximum(degradationAmount);

            if (currMaxStamina > minMaxStamina)
            {
                degradeTimer.Start();
            }
        }
    }

    void RegenOverTime()
    {
        if (CurrentStamina() < MaxStamina() && doRegen == true)
        {
            if (regenTimer.IsActive() == false && regenTimer.IsComplete() == false)
            {
                regenTimer.Start();
            } else if (regenTimer.IsComplete())
            {
                Replenish(regenAmount);
            }
        } else if (regenTimer.IsActive() || regenTimer.IsComplete())
        {
            regenTimer.Pause();
            regenTimer.Reset();
        }
    }

    void UpdateTimers()
    {
        degradeTimer.Count(Time.deltaTime);
        regenTimer.Count(Time.deltaTime);
    }

    void ReadTimers()
    {
        System.TimeSpan time = degradeTimer.GetFormattedTime();
        degradeTimerValue = $"{time.Seconds.ToString()}";
        
        time = regenTimer.GetFormattedTime();
        regenTimerValue = $"{time.Seconds.ToString()}";
    }

    public float MaxStamina()
    {
        return (currMaxStamina);
    }

    public float CurrentStamina()
    {
        return (currStamina);
    }

    public bool HaveStamina(float amount)
    {
        return (CurrentStamina() >= amount);
    }
}
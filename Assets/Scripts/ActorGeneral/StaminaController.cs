using UnityEngine;
using System.Collections;

public class StaminaController : MonoBehaviour
{

    enum StaminaState
    {
        Idle,
        Regen,
        Drain,
    }

    enum MaxState
    {
        Idle,
        Degrade,
    }

    [Header("Max Stamina", order= 1)]

    [SerializeField] float baseMaxStamina = 100f;
    [SerializeField] float minMaxStamina = 5f;

    MaxState maxState = MaxState.Idle;

        [Tooltip("How often max stamina should degrade, in seconds")]
    [SerializeField] float defaultDegradationInterval = 180;

        [Tooltip("How much max stamina is reduced per cycle")]
    [SerializeField] float defaultDegradationAmount = 5;


    [Header("Stamina Regeneration", order= 1)]

    [Tooltip("How fast stamina regenerates, in seconds")]
    [SerializeField] float defaultRegenInterval = 5f;

        [Tooltip("How much stamina regenerates per cycle")]
    [SerializeField] float defaultRegenAmount = 1f;
    StaminaState staminaState = StaminaState.Idle;

    [Header("", order = 1)]

    public float currMaxStamina {get; private set;}
    public float currStamina {get; private set;}

    void Awake()
    {
        RestoreMaximum(baseMaxStamina);
        Replenish(currMaxStamina);
    }

    void Update()
    {
        
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

    /// <summary>
    ///     Regenerate [amount] stamina back to this character's current stamina
    /// </summary>
    public void Replenish(float amount)
    {
        currStamina = Mathf.Clamp(currStamina + amount, currStamina + amount, currMaxStamina);
    }

    /// <summary>
    ///     Remove [amount] stamina from this character's current stamina 
    /// </summary>
    public void Expend(float amount)
    {
        currStamina = Mathf.Clamp(currStamina - amount, 0, currStamina - amount);
    }





    public string GetMaxState()
    {
        return(maxState.ToString());
    }

    void SetMaxState(MaxState newState)
    {
        Debug.Log($"maxState: {maxState} -> {newState}");
        maxState = newState;
    }

    public string GetStaminaState()
    {
        return (staminaState.ToString());
    }

    void SetStaminaState(StaminaState newState)
    {
        Debug.Log($"staminaState: {staminaState} -> {newState}");
        staminaState = newState;
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




    class TimedStaminaParams
    {
        public float amount;
        public float interval;
    }

    /// <summary>
    ///     Drain this character's stamina over time. Will keep looping until the player runs out or it is
    ///     stopped by calling Drain_Stop().
    ///     Expend [amount] stamina every [intervals] seconds
    /// </summary>
    public void Drain_Start(float amount, float intervals)
    {
        if (GetStaminaState() != "Drain")
        {
            Regen_Stop();
            SetStaminaState(StaminaState.Drain);
            StartCoroutine("DrainStaminaOverTime", new TimedStaminaParams { amount = amount, interval = intervals});
        }
    }

    public void Drain_Stop()
    {
        StopCoroutine("DrainStaminaOverTime");
        SetStaminaState(StaminaState.Idle);
    }

    IEnumerator DrainStaminaOverTime(TimedStaminaParams param)
    {
        if (HaveStamina(param.amount))
        {
            yield return new WaitForSeconds(param.interval);
            Expend(param.amount);
            StartCoroutine("DrainStaminaOverTime", param);
        } else
        {
            Debug.Log("Not enough stamina to drain. Coroutine cancelled");
            Drain_Stop();
        }
    }

    /// <summary>
    ///     Degrade this characters maximum stamina over time. Will loop until current maximum has reached minimum levels
    ///     or it is stopped by calling DegradeMaximum_Stop().
    ///     Reduce current max stamina by [amount] every [intervals] seconds
    /// </summary>
    public void DegradeMaximum_Start(float amount, float intervals)
    {
        Debug.Log("Called!");
        if (GetMaxState() != "Degrade")
        {
            SetMaxState(MaxState.Degrade);
            StartCoroutine("DegradeOverTime",new TimedStaminaParams { amount = amount, interval = intervals});
        }
    }

    /// <summary>
    ///     Degrade this characters maximum stamina over time. Will loop until current maximum has reached minimum levels
    ///     or it is stopped by calling DegradeMaximum_Stop().
    ///     Uses the character's default [amount] and [interval] values
    /// </summary>
    public void DegradeMaximum_Start()
    {
        DegradeMaximum_Start(defaultDegradationAmount, defaultDegradationInterval);
    }

    public void DegradeMaximum_Stop()
    {
        StopCoroutine("DegradeOverTime");
        SetMaxState(MaxState.Idle);
    }

    IEnumerator DegradeOverTime(TimedStaminaParams param)
    {
        if (currMaxStamina > minMaxStamina)
        {
            yield return new WaitForSeconds(param.interval);
            ReduceMaximum(param.amount);
            StartCoroutine("DegradeOverTime", param);
        } else
        {
            Debug.Log("Stamina cap has reached minimum levels. Coroutine cancelled");
            DegradeMaximum_Stop();
        }
    }

    /// <summary>
    ///     Regenerate current stamina over time. Will loop until current stamina is maxed out or 
    ///     Regen_Stop() is called.
    ///     Regenerate [amount] current stamina every [intervals] seconds
    /// </summary>
    public void Regen_Start(float amount, float intervals)
    {
        if (GetStaminaState() != "Regen")
        {
            Drain_Stop();
            SetStaminaState(StaminaState.Regen);
            StartCoroutine("RegenOverTime", new TimedStaminaParams{ amount = amount, interval = intervals });
        }
    }

    /// <summary>
    ///     Regenerate current stamina over time. Will loop until current stamina is maxed out or 
    ///     Regen_Stop() is called.
    ///     Uses the character's default [amount ] and [interval] values
    /// </summary>
    public void Regen_Start()
    {
        Regen_Start(defaultRegenAmount, defaultRegenInterval);
    }

    public void Regen_Stop()
    {
        StopCoroutine("RegenOverTime");
        SetStaminaState(StaminaState.Idle);
    }

    IEnumerator RegenOverTime(TimedStaminaParams param)
    {
        if (CurrentStamina() < MaxStamina())
        {
            yield return new WaitForSeconds(param.interval);
            Replenish(param.amount);
            StartCoroutine("RegenOverTime", param);
        } else
        {
            Debug.Log("Stamina has reached maximum levels. Coroutine cancelled");
            Regen_Stop();
        }
    }
}
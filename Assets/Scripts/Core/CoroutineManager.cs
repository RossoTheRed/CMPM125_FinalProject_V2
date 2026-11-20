using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager _instance;

    public static CoroutineManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple CoroutineManager scripts. Bad!");
        }
        else
        {
            _instance = this;
        }
    }




    public void Run(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void Stop(IEnumerator coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    
}

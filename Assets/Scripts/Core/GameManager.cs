using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    private static GameManager _instance;
    public static GameManager Instance {  get
        {
            if (_instance == null)
                _instance = new GameManager();
            return _instance;
        }
    }

    public enum GameState
    {
        MAINMENU,
        INGAME,
        PAUSE,
        GAMEOVER
    }

    // ChangeGameState
    // Handles changing the game state, sends out signals when doing so through eventbus
    public void ChangeGameState(GameState stateChange)
    {
        //Debug.Log(stateChange);
        switch (stateChange)
        {
            case GameState.MAINMENU:
                EventBus.Instance.DoMainMenu();
                break;
            case GameState.INGAME:
                EventBus.Instance.DoGameStart();
                break;
            case GameState.PAUSE:
                EventBus.Instance.DoGamePause();
                break;
            case GameState.GAMEOVER:
                EventBus.Instance.DoGameOver();
                break;
            default:
                Debug.LogError("Invalid GameState Transition");
                break;
        }
        state = stateChange;
    }

    // ChangeScene
    // Handles switching between scenes (might need scene manager)
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public GameState state;

    // Note switch this initialization to be based on saved data
    //public OptionsData gameOptionsData = SaveSystem.GetOptions();
    public GameObject player;


    // place for any managers


    public GameManager()
    {
        
    }


}
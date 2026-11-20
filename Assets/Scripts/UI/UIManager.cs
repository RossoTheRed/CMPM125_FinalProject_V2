using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // either make this a list or enum maybe? either way acts as like a state
    [SerializeField] private Dictionary<string, GameObject> localMenus;
    public KeyCode nextKey;
    public KeyCode[] menuKeys; // should be linked to menus, so 1st key opens 1st menu, etc
    public string currentMenu = "None";
    private static UIManager _instance;

    public static UIManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // handoff before destruction
            _instance.RecieveMenus(localMenus);
            
            Destroy(gameObject);
            Debug.LogWarning("Multiple UI managers, no!");
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        // Dont destroy is not on the outside incase you destroy the gameobject
        // This is for if we incorporate saves
        /*if (localMenus.ContainsKey("OptionsMenu")) {
            localMenus["OptionsMenu"].GetComponent<OptionsMenu>().ApplySettings(GameManager.Instance.gameOptionsData);
        }*/
    }

    private void RecieveMenus(Dictionary<string, GameObject> menus)
    {
        localMenus.Clear();
        localMenus = menus;
    }

    public string GetCurrentMenu()
    {
        return currentMenu;
    }

    

    public void EnableMenu(string name)
    {
        if (!localMenus.TryGetValue(name, out GameObject menu)) return;
        if (!menu.TryGetComponent(out Canvas canvas)) return;
        currentMenu = name;
        menu.SetActive(true);
        canvas.enabled = true;
    }

    public void DisableMenu(string name)
    {
        if (!localMenus.TryGetValue(name, out GameObject menu)) return;
        if (!menu.TryGetComponent(out Canvas canvas)) return;

        canvas.enabled = false;
        menu.SetActive(false);
    }

    public void SwitchMenu(string from, string to)
    {
        // Checks if both exist
        if (!localMenus.ContainsKey(from) || !localMenus.ContainsKey(to)) return;
        EnableMenu(to);
        DisableMenu(from);
    }
}

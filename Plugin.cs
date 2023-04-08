using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace swedish_pizza;

[BepInPlugin(GUID, NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    private const string GUID = "raisin.postgame.swedish_pizza";
    private const string NAME = "Swedish Pizza";
    private const string VERSION = "0.0.1";

    private void Awake()
    {
        Logger.LogInfo($"Plugin {GUID} is loaded!");

        GameObject inputManager = new GameObject("Input Manager");
        inputManager.AddComponent<InputManager>();
    }
}

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Saver.Save("test");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Saver.Load("test");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        if (FindObjectsOfType<GameManager>().Length > 1) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationQuit()
    {
        //Save the game when the player exits the application
        SaveSystem.Save();
    }
}

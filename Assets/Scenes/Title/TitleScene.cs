using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1.0f;
    }

    #region Exit Menu
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}

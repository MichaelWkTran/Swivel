using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static bool m_isApplicationStarted = true;
    [SerializeField] UnityEngine.Audio.AudioMixer m_audioMixer;

    void Start()
    {
        if (FindObjectsOfType<GameManager>().Length > 1) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        if (!m_isApplicationStarted) return;
        
        //--------------------------------------------------------------------------------------------------------------------------------
        if (SaveSystem.m_Data.m_sfxVolume > -100.0f) m_audioMixer.SetFloat("sfxVolume", SaveSystem.m_Data.m_sfxVolume);
        if (SaveSystem.m_Data.m_musicVolume > -100.0f) m_audioMixer.SetFloat("musicVolume", SaveSystem.m_Data.m_musicVolume);
        if (SaveSystem.m_Data.m_qualityLevel >= 0.0f) QualitySettings.SetQualityLevel(SaveSystem.m_Data.m_qualityLevel);
        if (SaveSystem.m_Data.m_dragSensitivity >= 0.0f) RotatableMesh.m_dragSensitivity = SaveSystem.m_Data.m_dragSensitivity;

        //--------------------------------------------------------------------------------------------------------------------------------
        m_isApplicationStarted = false;
    }

    void OnApplicationQuit()
    {
        //Save the game when the player exits the application
        SaveSystem.Save();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

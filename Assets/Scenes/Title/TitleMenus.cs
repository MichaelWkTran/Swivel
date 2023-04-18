using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//All code relating to the menus the player can access before the game starts including the title scene,
//settings scene, shop scene, and exit scene
public class TitleMenus : MonoBehaviour
{
    static bool m_isApplicationStarted = true;

    [Header("Title Screen")]
    [SerializeField] RectTransform m_titleScreen;
    [SerializeField] TMPro.TMP_Text m_highScoreText;

    [Header("Settings Screen")]
    [SerializeField] RectTransform m_settingsScreen;
    [SerializeField] UnityEngine.Audio.AudioMixer m_audioMixer;
    [SerializeField] Slider m_sfxVolumeSlider;
    [SerializeField] Slider m_musicVolumeSlider;
    [SerializeField] TMPro.TMP_Dropdown m_graphicsDropdown;

    void Start()
    {
        //Set the high score text on the title screen
        int highScore = SaveSystem.m_data.m_highScore;
        if (highScore > 0) m_highScoreText.text = "High Score: " + highScore.ToString();
        else m_highScoreText.gameObject.SetActive(false);

        //Set settings in settings menu
        if (m_isApplicationStarted)
        {
            m_audioMixer.SetFloat("sfxVolume", SaveSystem.m_data.m_sfxVolume);
            m_audioMixer.SetFloat("musicVolume", SaveSystem.m_data.m_musicVolume);
            QualitySettings.SetQualityLevel(SaveSystem.m_data.m_qualityLevel);
            m_isApplicationStarted = false;
        }

        {
            float sfxVolumeValue; m_audioMixer.GetFloat("sfxVolume", out sfxVolumeValue);
            m_sfxVolumeSlider.value = sfxVolumeValue;
        }
        {
            float musicVolumeValue; m_audioMixer.GetFloat("musicVolume", out musicVolumeValue);
            m_musicVolumeSlider.value = musicVolumeValue;
        }
        m_graphicsDropdown.value = QualitySettings.GetQualityLevel();
    }


    #region Title Menu
    public void StartGame()
    {
        
    }
    #endregion

    #region Settings Menu
    public void SetSFXVolume(float _value)
    {
        m_audioMixer.SetFloat("sfxVolume", SaveSystem.m_data.m_sfxVolume = _value);
    }

    public void SetMusicVolume(float _value)
    {
        m_audioMixer.SetFloat("musicVolume", SaveSystem.m_data.m_musicVolume = _value);
    }

    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(SaveSystem.m_data.m_qualityLevel = _qualityIndex);
    }
    #endregion

    #region Credits Menu
    public void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }
    #endregion Credits Menu

    #region Exit Menu
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}

using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    static bool m_isApplicationStarted = true;

    [Header("Settings Screen")]
    [SerializeField] UnityEngine.Audio.AudioMixer m_audioMixer;
    [SerializeField] Slider m_sfxVolumeSlider;
    [SerializeField] Slider m_musicVolumeSlider;
    [SerializeField] TMPro.TMP_Dropdown m_graphicsDropdown;

    [Header("Shop Screen")]
    [SerializeField] ScrollRect m_shopScrollRect;
    [SerializeField] Transform m_shopContents;


    void Start()
    {
        Time.timeScale = 1.0f;

        Instantiate(GameMode.m_EnviromentPrefab);

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

    #region Shop Menu
    public void TabsOnToggle(RectTransform _toggledContent)
    {
        foreach(Transform child in m_shopContents) child.gameObject.SetActive(false);

        if (_toggledContent == null) return;
        _toggledContent.gameObject.SetActive(true);
        m_shopScrollRect.content = _toggledContent;
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
}

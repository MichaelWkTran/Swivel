using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    static bool m_isApplicationStarted = true;

    [Header("Level Select Screen")]
    [HideInInspector] public LevelButton[] m_LevelButtons;

    [Header("Settings Screen")]
    [SerializeField] UnityEngine.Audio.AudioMixer m_audioMixer;
    [SerializeField] Slider m_sfxVolumeSlider;
    [SerializeField] Slider m_musicVolumeSlider;
    [SerializeField] TMPro.TMP_Dropdown m_graphicsDropdown;


    void Start()
    {
        Time.timeScale = 1.0f;

        //Update Enviroment
        UpdateEnviroment(Enviroment.m_CurrentEnviroment);
        
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

    public void UpdateEnviroment(Enviroment _newEnviromentPrefab)
    {
        //Destroy existing enviroments in the world
        var enviroments = FindObjectsOfType<Enviroment>();
        foreach (Enviroment enviroment in enviroments) Destroy(enviroment.gameObject);

        //Set Background
        Enviroment.m_CurrentEnviroment = _newEnviromentPrefab;
        Instantiate(Enviroment.m_CurrentEnviroment);

        //Set Level buttons
        m_LevelButtons = FindObjectsOfType<LevelButton>(true);
        foreach (var levelButton in m_LevelButtons)
        {
            Color enviromentColour = Enviroment.m_CurrentEnviroment.m_colour;
            levelButton.ShapeImage.color = enviromentColour;

            Vector3 backgroundHSV;
            Color.RGBToHSV(enviromentColour, out backgroundHSV.x, out backgroundHSV.y, out backgroundHSV.z);
            backgroundHSV.x = Mathf.Repeat(backgroundHSV.x + 0.15f, 1.0f);
            backgroundHSV.y -= 0.6f;
            enviromentColour = Color.HSVToRGB(backgroundHSV.x, backgroundHSV.y, backgroundHSV.z);


            levelButton.GetComponent<Image>().color = enviromentColour;
        }
    }

    public void UpdateGameUI(GameUI _newGameUI)
    {
        GameUI.m_CurrentGameUI = _newGameUI;
    }

    #region Title Menu
    public void StartGame()
    {

    }
    #endregion

    #region Shop Menu
    
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

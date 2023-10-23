using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    static bool m_isApplicationStarted = true;

    [Header("Level Select Screen")]
    [SerializeField] RectTransform m_levelButtonsContent;
    [HideInInspector] public LevelButton[] m_levelButtons;

    [Header("Settings Screen")]
    [SerializeField] UnityEngine.Audio.AudioMixer m_audioMixer;
    [SerializeField] Slider m_sfxVolumeSlider;
    [SerializeField] Slider m_musicVolumeSlider;
    [SerializeField] TMPro.TMP_Dropdown m_graphicsDropdown;


    void Start()
    {
        Time.timeScale = 1.0f;

        //Set Levels Unlocked
        m_levelButtons = new LevelButton[m_levelButtonsContent.childCount];
        for (int i = 0; i < m_levelButtonsContent.childCount; i++)
        {
            LevelButton levelButton = m_levelButtonsContent.GetChild(i).GetComponent<LevelButton>();
            m_levelButtons[i] = levelButton;
            //if (i > SaveSystem.m_Data.m_unlockedLevels) levelButton.GetComponent<Button>().interactable = false;
        }

        //Update Enviroment
        UpdateEnviroment(Enviroment.m_CurrentEnviroment);

        //Set settings in settings menu
        if (m_isApplicationStarted)
        {
            m_audioMixer.SetFloat("sfxVolume", SaveSystem.m_Data.m_sfxVolume);
            m_audioMixer.SetFloat("musicVolume", SaveSystem.m_Data.m_musicVolume);
            QualitySettings.SetQualityLevel(SaveSystem.m_Data.m_qualityLevel);
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

    public void UpdateSpriteGroup(SpriteGroup _newSpriteGroup)
    {
        SpriteGroup.m_CurrentSpriteGroup = _newSpriteGroup;
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
        foreach (var levelButton in m_levelButtons)
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
        m_audioMixer.SetFloat("sfxVolume", SaveSystem.m_Data.m_sfxVolume = _value);
    }

    public void SetMusicVolume(float _value)
    {
        m_audioMixer.SetFloat("musicVolume", SaveSystem.m_Data.m_musicVolume = _value);
    }

    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(SaveSystem.m_Data.m_qualityLevel = _qualityIndex);
    }
    #endregion

    #region Credits Menu
    public void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }
    #endregion Credits Menu
}

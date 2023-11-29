using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [Header("Level Select Screen")]
    [SerializeField] RectTransform m_levelButtonsContent;
    [HideInInspector] public LevelButton[] m_levelButtons;

    void Start()
    {
        Time.timeScale = 1.0f;

        //Set Levels Unlocked
        m_levelButtons = new LevelButton[m_levelButtonsContent.childCount];
        for (int i = 0; i < m_levelButtonsContent.childCount; i++)
        {
            LevelButton levelButton = m_levelButtonsContent.GetChild(i).GetComponent<LevelButton>();
            m_levelButtons[i] = levelButton;
            RotatableMesh.m_shapeColour = levelButton.m_shapeColour;
            //if (i > SaveSystem.m_Data.m_unlockedLevels) levelButton.GetComponent<Button>().interactable = false;
        }

        //Update Enviroment
        UpdateEnviroment(Enviroment.m_CurrentEnviroment);
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
        //Enviroment.m_CurrentEnviroment = _newEnviromentPrefab;
        Instantiate(Enviroment.m_CurrentEnviroment);

        //Set Level buttons
        foreach (var levelButton in m_levelButtons)
        {
            //Get the current colour of the button
            {
                levelButton.ShapeImage.color = levelButton.m_shapeColour;
                //levelButton.ShapeImage.color *= Enviroment.m_CurrentEnviroment.m_colour;
            }

            //Modify the colour of the button background
            {
                //Vector3 buttonBgHSV; Color.RGBToHSV(Enviroment.m_CurrentEnviroment.m_colour, out buttonBgHSV.x, out buttonBgHSV.y, out buttonBgHSV.z);
                //buttonBgHSV.x = Mathf.Repeat(buttonBgHSV.x + 0.15f, 1.0f);
                //buttonBgHSV.y -= 0.6f;
                //Color buttonBgColour = Color.HSVToRGB(buttonBgHSV.x, buttonBgHSV.y, buttonBgHSV.z);
                //
                //levelButton.BackgroundImage.color *= buttonBgColour;
            }
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

    #region Credits Menu
    public void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }
    #endregion Credits Menu
}

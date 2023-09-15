using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static ObjectGUIDGroup m_guidGroup => (ObjectGUIDGroup)AssetDatabase.LoadAssetAtPath
    (
        "",//"Assets/Scenes/Level/Sprite Groups/Sprite GUID Groups.asset",
        typeof(ObjectGUIDGroup)
    ); //What guid group is this object apart of

    static GameUI m_currentGameUI = null; //Current game UI
    public static GameUI m_CurrentGameUI
    {
        get
        {
            if (m_currentGameUI == null) m_currentGameUI = (GameUI)m_guidGroup.m_ObjectGUIDs[0].m_assetReference;
            return m_currentGameUI;
        }

        set { m_currentGameUI = value; }
    }

    public Canvas m_mainCanvas; //The canvas that shows the main UI
    public Canvas m_gameOverCanvas; //The canvas that shows the game over screen
    public Canvas m_pauseCanvas; //The canvas that shows the pause screen
    public Image m_currentImage; //The image that shows what sprite on the rotatable mesh that the player has to match
    public TMPro.TMP_Text m_scoreValueText; //The text that displays the current score of the game
    public Slider m_timerBar; //The UI that shows how much time is left
    public Gradient m_timerBarGradient; //The colour of the timer bar depending on how much time is left

    GameMode m_gameMode;

    void Start()
    {
        m_gameMode = FindObjectOfType<GameMode>();
    }

    public void OnPauseButtonClick()
    {
        m_gameMode.Pause();
    }

    public void OnResumeButtonClick()
    {
        m_gameMode.UnPause();
    }

    public void OnShareButtonClick()
    {
        m_gameMode.ShareHighScore();
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static ObjectGUIDGroup m_guidGroup => Resources.Load<ObjectGUIDGroup>("UI Themes GUID Group"); //What guid group is this object apart of
    static GameUI m_currentGameUI = null; //Current game UI
    public static GameUI m_CurrentGameUI
    {
        get
        {
            if (m_currentGameUI == null) m_currentGameUI = m_guidGroup.GetObjectByGUID<GameObject>
                    (SaveSystem.m_Data.m_currentUIThemeGUID)?.GetComponent<GameUI>();
            return m_currentGameUI;
        }

        set
        {
            m_currentGameUI = value;

            if (m_currentGameUI == null) return;
            SaveSystem.m_Data.m_currentUIThemeGUID = m_guidGroup.GetGUIDFromObject(m_currentGameUI.gameObject);
        }
    }

    public Canvas m_mainCanvas; //The canvas that shows the main UI
    public Canvas m_gameOverCanvas; //The canvas that shows the game over screen
    public Canvas m_pauseCanvas; //The canvas that shows the pause screen
    public Image m_currentImage; //The image that shows what sprite on the rotatable mesh that the player has to match
    public TMPro.TMP_Text m_scoreValueText; //The text that displays the current score of the game
    public TMPro.TMP_Text m_addedScoreText; //The text that displays the score that the player earned in a round
    public Slider m_timerBar; //The UI that shows how much time is left
    public Gradient m_timerBarGradient; //The colour of the timer bar depending on how much time is left
    public TMPro.TMP_Text m_resultsText; //The text that displays the results of a game in the gameover screen
    public TMPro.TMP_Text m_gameOverScore; //The score shown in the game over screen
    public TMPro.TMP_Text m_gameOverHighScore; //The high score shown in the game over screen
    public TMPro.TMP_Text m_gameOverMoney; //The money earned shown in the game over screen

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

    public void ShowAddedScore(float _addedScore)
    {
        TMPro.TMP_Text addedScoreText = Instantiate(m_addedScoreText, m_mainCanvas.transform);

        addedScoreText.gameObject.SetActive(true);
        {
            Vector2 canvasSize = m_mainCanvas.GetComponent<RectTransform>().rect.size / 2.0f;
            addedScoreText.GetComponent<RectTransform>().anchoredPosition = Random.insideUnitCircle * Mathf.Min(canvasSize.x, canvasSize.y) * 0.6f;
        }
        addedScoreText.transform.localScale = Vector3.zero;
        m_addedScoreText.text = "+" + _addedScore.ToString();

        IEnumerator ScoreTextAnimation()
        {
            LeanTween.scale(addedScoreText.gameObject, Vector3.one, 0.5f)
                .setEaseInExpo().setEaseOutBounce();

            Color initalColour = addedScoreText.color; initalColour[3] = 0.0f;
            LeanTween.value(addedScoreText.gameObject, initalColour, addedScoreText.color, 0.2f)
                .setEaseInExpo()
                .setOnUpdate((Color _value) => { addedScoreText.color = _value; });

            LeanTween.moveY(addedScoreText.gameObject, addedScoreText.transform.position.y + 50.0f, 2.0f);

            yield return new WaitForSeconds(1.5f);

            LeanTween.value(addedScoreText.gameObject, addedScoreText.color, initalColour, 0.5f)
                .setEaseInExpo()
                .setOnUpdate((Color _value) => { addedScoreText.color = _value; });

            yield return new WaitForSeconds(0.5f);

            Destroy(addedScoreText.gameObject);
        }

        StartCoroutine(ScoreTextAnimation());
    }
}

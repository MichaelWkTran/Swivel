using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    public float m_score; //The score of the current game
    public uint m_round { get; private set; } = 1;
    [SerializeField] float m_startTime;
    [SerializeField] float m_timeDecreaseRate;
    [SerializeField] float m_minTime; //The fastest time a round can have
    public bool m_paused { get; private set; } = false; //Whether the game is currently paused
    RotatableMesh m_rotatableMesh;

    [Header("UI")]
    [SerializeField] TMPro.TMP_Text m_scoreValueText; //The text that displays the current score of the game
    [SerializeField] TMPro.TMP_Text m_roundValueText; //The text that displays the round the player is in
    public Image m_currentImage;
    [SerializeField] Slider m_timerBar; //The UI that shows how much time is left
    [SerializeField] Gradient m_timerBarGradient;
    [SerializeField] Canvas m_mainCanvas;
    [SerializeField] Canvas m_gameOverCanvas;
    [SerializeField] Canvas m_pauseCanvas;


    void Start()
    {
        m_rotatableMesh = FindObjectOfType<RotatableMesh>();
        m_roundValueText.text = m_round.ToString();

        //Randomise Image
        {
            IEnumerator Coroutine()
            {
                //Wait until the rotatable mesh have set a target face
                yield return new WaitUntil(() => m_rotatableMesh.m_targetFaceIndex >= 0);

                //Randomize image
                RandomizeImage();
            }
            StartCoroutine(Coroutine());
        }
        
        //Setup timer
        m_timerBar.value = m_timerBar.maxValue = m_startTime;
    }

    void Update()
    {
        //Pause/Unpause the game
        if (Input.GetKeyDown(KeyCode.Escape)) if (!m_paused) Pause(); else UnPause();

        //
        if (m_paused) return;

        //
        if (m_round == 1) return;
        
        //Update Timer
        m_timerBar.value -= Time.deltaTime;
        m_timerBar.fillRect.GetComponent<Image>().color = m_timerBarGradient.Evaluate(m_timerBar.value / m_timerBar.maxValue);

        //Trigger Game Over when the timer runs out
        if (m_timerBar.value <= 0.0f) GameOver();
    }

    void OnApplicationPause(bool _pause)
    {
        if (!_pause) return;

        //Pause the game when the application is not in focus
        if (!m_paused) Pause();
    }

    void OnApplicationQuit()
    {
        //Save the game when the player exits the application
        SaveSystem.Save();
    }

    public void WinRound()
    {
        //Check whether the player can win this round
        if (m_rotatableMesh.GetCurrentFace().sprite != m_currentImage.sprite) return;

        //Update current round
        m_round++;
        m_roundValueText.text = m_round.ToString();

        //Randomise Image
        RandomizeImage();

        //Add Score
        m_score += Mathf.Round((m_timerBar.value / m_timerBar.maxValue) * 100.0f) * 10.0f;
        m_scoreValueText.text = m_score.ToString();

        //Reset Timer
        m_timerBar.maxValue -= m_timeDecreaseRate;
        if (m_timerBar.maxValue < m_minTime) m_timerBar.maxValue = m_minTime;
        m_timerBar.value = m_timerBar.maxValue;
    }

    public void GameOver()
    {
        //Update High Score
        if (m_score > SaveSystem.m_data.m_highScore)
        {
            //Update Score UI and HighScore
            SaveSystem.m_data.m_highScore = (int)m_score;
        }
        else
        {
            //Update Score UI
            
        }

        //Update Money
        //SaveSystem.m_data.m_money += m_score;

        m_mainCanvas.gameObject.SetActive(false);
        m_gameOverCanvas.gameObject.SetActive(true);
    }

    public void Pause()
    {
        m_paused = true;
        m_pauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void UnPause()
    {
        m_paused = false;
        m_pauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void GoToTitle()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        //GoToTitle();
        //Menus.m_playOnAwake = true;
    }

    public void ShareHighScore()
    {
        //This function takes a screenshot and shares it. Used for sharing highscores.

        IEnumerator TakeScreenshotAndShare()
        {
            yield return new WaitForEndOfFrame();

#if UNITY_ANDROID || UNITY_IOS
            //Takes the screen shot
            Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();

            string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
            File.WriteAllBytes(filePath, screenShot.EncodeToPNG());

            //Destroy screenshot to avoid memory leaks
            Destroy(screenShot);

            new NativeShare().AddFile(filePath)
                .SetSubject("Awesome score from Overflow")
                .SetText(SaveSystem.m_data.m_highScore.ToString() + " points!" + "I'm on a roll!")
                .SetUrl("https://birdbraingamesdev.itch.io/").Share();
#endif
        }

        TakeScreenshotAndShare();
    }

    public void RandomizeImage()
    {
        int selectedFaceIndex = Random.Range(0, m_rotatableMesh.m_FaceTextures.Length);
        if (m_rotatableMesh.m_targetFaceIndex == selectedFaceIndex)
        {
            selectedFaceIndex++;
            if (m_rotatableMesh.m_FaceTextures.Length == selectedFaceIndex) selectedFaceIndex -= 2;
        }
        m_currentImage.sprite = m_rotatableMesh.m_FaceTextures[selectedFaceIndex].sprite;
    }
}

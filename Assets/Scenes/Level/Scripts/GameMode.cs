using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    public float m_score; //The score of the current game
    public uint m_round { get; private set; } = 1; //How many rounds have the player completed
    [SerializeField] float m_startTime; //How much time the player have when they start the game
    [SerializeField] float m_timeDecreaseRate; //How much max time is taken away evey time a round is completed
    [SerializeField] float m_minTime; //The fastest time a round can have
    public bool m_paused { get; private set; } = false; //Whether the game is currently paused
    RotatableMesh m_rotatableMesh;

    public static RotatableMesh m_rotatableMeshPrefab;
    static Enviroment m_enviromentPrefab;
    public static Enviroment m_EnviromentPrefab
    {
        get { return (m_enviromentPrefab != null) ? m_enviromentPrefab : (Enviroment)AssetDatabase.LoadAssetAtPath("Assets/Enviroments/Default Enviroment.prefab", typeof(Enviroment)); }
        set { m_enviromentPrefab = value; }
    }

    [Header("UI")]
    [SerializeField] TMPro.TMP_Text m_scoreValueText; //The text that displays the current score of the game
    [SerializeField] TMPro.TMP_Text m_roundValueText; //The text that displays the round the player is in
    public Image m_currentImage; //The image that shows what sprite on the rotatable mesh that the player has to match
    [SerializeField] Slider m_timerBar; //The UI that shows how much time is left
    [SerializeField] Gradient m_timerBarGradient; //The colour of the timer bar depending on how much time is left
    [SerializeField] Canvas m_mainCanvas; //The canvas that shows the main UI
    [SerializeField] Canvas m_gameOverCanvas; //The canvas that shows the game over screen
    [SerializeField] Canvas m_pauseCanvas; //The canvas that shows the pause screen

    void Start()
    {
        //
        Time.timeScale = 1.0f;

        //Setup Enviroment and Camera
        Enviroment enviroment = Instantiate(m_EnviromentPrefab);
        enviroment.SetEnviromentAndCamera();

        //
        if (m_rotatableMeshPrefab == null) m_rotatableMeshPrefab = (RotatableMesh)AssetDatabase.LoadAssetAtPath("Assets/Scenes/Level/Shapes/Cube.prefab", typeof(RotatableMesh));
        m_rotatableMesh = Instantiate(m_rotatableMeshPrefab);
        m_rotatableMesh.m_faceColour = enviroment.m_colour;
        m_rotatableMesh.SetColour();

        //
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

        //Dont update hte game when it is paused
        if (m_paused) return;

        //Dont update when the player hasn't finished their first round
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
        //Update Money
        //SaveSystem.m_data.m_money += m_score;

        m_mainCanvas.gameObject.SetActive(false);
        m_gameOverCanvas.gameObject.SetActive(true);
        enabled = false;
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
}
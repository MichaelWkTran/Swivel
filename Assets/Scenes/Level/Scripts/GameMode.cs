using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEditor;

public class GameMode : MonoBehaviour
{
    [Header("Gameplay")]
    public float m_score; //The score of the current game
    public uint m_round { get; private set; } = 1; //How many rounds have the player completed
    [SerializeField] float m_startTime; //How much time the player have when they start the game
    [SerializeField] float m_timeDecreaseRate; //How much max time is taken away evey time a round is completed
    [SerializeField] float m_minTime; //The fastest time a round can have

    public bool m_paused { get; private set; } = false; //Whether the game is currently paused
    RotatableMesh m_rotatableMesh; //The rotatable mesh that the player interacts with
    GameUI m_gameUI; //Where all the UI elements are stored

    [Header("Graphics")]
    [SerializeField] float m_winFlashIntensity;
    [SerializeField] float m_winFlashTime;

    [Header("Static Variables")]
    static RotatableMesh m_rotatableMeshPrefab; public static RotatableMesh m_RotatableMeshPrefab
    {
        get
        {
            if (m_rotatableMeshPrefab == null) m_rotatableMeshPrefab = AssetDatabase.LoadAssetAtPath<RotatableMesh>("Assets/Scenes/Level/Shapes/Cube.prefab");
            return m_rotatableMeshPrefab;
        }
        set { m_rotatableMeshPrefab = value; }
    }
    public static uint m_currentLevelIndex = 0;


    void Start()
    {
        //Ensure that the game starts with proper time scale
        Time.timeScale = 1.0f;

        //Setup Shape, Enviroment, and UI
        Enviroment enviroment = Instantiate(Enviroment.m_CurrentEnviroment);
        m_rotatableMesh = Instantiate(m_RotatableMeshPrefab);
        m_gameUI = Instantiate(GameUI.m_CurrentGameUI); m_gameUI.transform.parent = transform; m_gameUI.gameObject.name = "UI";

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
        m_gameUI.m_timerBar.value = m_gameUI.m_timerBar.maxValue = m_startTime;

        //Start Director
        GetComponent<PlayableDirector>().Play();
    }

    void Update()
    {
        if (!enabled) return;

        //Pause/Unpause the game
        if (Input.GetKeyDown(KeyCode.Escape)) if (!m_paused) Pause(); else UnPause();

        //Dont update hte game when it is paused
        if (m_paused) return;

        //Dont update when the player hasn't finished their first round
        if (m_round == 1) return;

        //Update Timer
        m_gameUI.m_timerBar.value -= Time.deltaTime;
        m_gameUI.m_timerBar.fillRect.GetComponent<Image>().color = m_gameUI.m_timerBarGradient.Evaluate(m_gameUI.m_timerBar.value / m_gameUI.m_timerBar.maxValue);

        //Trigger Game Over when the timer runs out
        if (m_gameUI.m_timerBar.value <= 0.0f) GameOver();
    }

    void OnApplicationPause(bool _pause)
    {
        if (!enabled) return;
        if (!_pause) return;

        //Pause the game when the application is not in focus
        if (!m_paused) Pause();
    }

    public void RandomizeImage()
    {
        //Get the radomly selected silhouette from the rotable mesh and apply it to the UI
        int selectedFaceIndex = Random.Range(0, m_rotatableMesh.m_faceTextures.Length);
        if (m_rotatableMesh.m_targetFaceIndex == selectedFaceIndex)
        {
            selectedFaceIndex++;
            if (m_rotatableMesh.m_faceTextures.Length == selectedFaceIndex) selectedFaceIndex -= 2;
        }
        m_gameUI.m_currentImage.sprite = m_rotatableMesh.m_faceTextures[selectedFaceIndex].sprite;
    }

    public void WinRound()
    {
        //Check whether the player can win this round
        if (m_rotatableMesh.GetCurrentFace().sprite != m_gameUI.m_currentImage.sprite) return;

        //Play Win Particles
        m_rotatableMesh.m_winParticles.Play();

        //Make the shape flash
        {
            Bloom bloom; Volume volume = FindObjectOfType<Enviroment>().GetComponent<Volume>();
            if (volume.profile.TryGet(out bloom)) LeanTween.value(gameObject, (float _value) => { bloom.intensity.value = _value; },bloom.intensity.value, bloom.intensity.value + m_winFlashIntensity, m_winFlashTime).setEasePunch();
        }

        //Update current round
        m_round++;
        
        //Randomise Image
        RandomizeImage();

        //Add Score
        float addedScore = Mathf.Round((m_gameUI.m_timerBar.value / m_gameUI.m_timerBar.maxValue) * 100.0f) * 10.0f;
        m_score += addedScore;
        m_gameUI.m_scoreValueText.text = m_score.ToString();
        m_gameUI.ShowAddedScore(addedScore);

        //Reset Timer
        m_gameUI.m_timerBar.maxValue -= m_timeDecreaseRate;
        if (m_gameUI.m_timerBar.maxValue < m_minTime) m_gameUI.m_timerBar.maxValue = m_minTime;
        m_gameUI.m_timerBar.value = m_gameUI.m_timerBar.maxValue;
    }

    public void GameOver()
    {
        //Save Data
        bool newHighScore = SaveSystem.m_Data.m_highScores[m_currentLevelIndex] < m_score;
        if (newHighScore) SaveSystem.m_Data.m_highScores[m_currentLevelIndex] = m_score;
        
        float earnedMoney = Mathf.Round(m_score / 100.0f);
        SaveSystem.m_Data.m_money += earnedMoney;

        //Update UI
        //m_gameUI.m_resultsText.text =
        //    $"Score: {m_score}\n" +
        //    $"Best: {SaveSystem.m_Data.m_highScores[m_currentLevelIndex]}\n";// + 
        //    //$"Earned: {}";
        m_gameUI.m_gameOverScore.text = m_score.ToString();
        m_gameUI.m_gameOverHighScore.text = SaveSystem.m_Data.m_highScores[m_currentLevelIndex].ToString();
        m_gameUI.m_gameOverMoney.text = earnedMoney.ToString();

        //Enable Game Over Canvas
        m_gameUI.m_gameOverCanvas.gameObject.SetActive(true);

        //Enable GameOverTimeline Script
        GetComponent<GameOverTimeline>().enabled = true;
        
        //Disable this script to disable gameplay
        enabled = false;
    }

    public void Pause()
    {
        if (!enabled) return;

        m_paused = true;
        m_gameUI.m_pauseCanvas.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void UnPause()
    {
        if (!enabled) return;

        m_paused = false;
        m_gameUI.m_pauseCanvas.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ShareHighScore()
    {
        //This function takes a screenshot and shares it. Used for sharing highscores.

        IEnumerator TakeScreenshotAndShare()
        {
            yield return new WaitForEndOfFrame();

//#if UNITY_ANDROID || UNITY_IOS
//            //Takes the screen shot
//            Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
//            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
//            screenShot.Apply();
//
//            string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
//            File.WriteAllBytes(filePath, screenShot.EncodeToPNG());
//
//            //Destroy screenshot to avoid memory leaks
//            Destroy(screenShot);
//
//            new NativeShare().AddFile(filePath)
//                .SetSubject("Awesome score from Overflow")
//                .SetText(SaveSystem.m_Data.m_highScore.ToString() + " points!" + "I'm on a roll!")
//                .SetUrl("https://birdbraingamesdev.itch.io/").Share();
//#endif
        }

        TakeScreenshotAndShare();
    }
}
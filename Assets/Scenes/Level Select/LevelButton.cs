using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] RotatableMesh m_rotatableMeshPrefab; public RotatableMesh m_RotatableMeshPrefab { get { return m_rotatableMeshPrefab; } }
    [SerializeField] Image m_shapeImage; public Image ShapeImage { get { return m_shapeImage; } }
    [SerializeField] Image m_backgroundImage; public Image BackgroundImage { get { return m_backgroundImage; } }
    [SerializeField] TMPro.TMP_Text m_highScoreText;

    void Start()
    {
        float highScore = SaveSystem.m_Data.m_highScores[transform.GetSiblingIndex()];
        if (highScore > 0) m_highScoreText.text = highScore.ToString();
    }

    public void LoadLevel()
    {
        GameMode.m_RotatableMeshPrefab = m_rotatableMeshPrefab;
        GameMode.m_currentLevelIndex = (uint)transform.GetSiblingIndex();
        GetComponent<ChangeScene>().LoadScene("Level");
    }
}

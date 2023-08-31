using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] RotatableMesh m_rotatableMeshPrefab;
    [SerializeField] Image m_shapeImage; public Image ShapeImage { get { return m_shapeImage; } }
    [SerializeField] Image m_backgroundImage; public Image BackgroundImage { get { return m_backgroundImage; } }

    public void LoadLevel()
    {
        GameMode.m_RotatableMeshPrefab = m_rotatableMeshPrefab;
        GetComponent<ChangeScene>().LoadScene("Level");
    }
}

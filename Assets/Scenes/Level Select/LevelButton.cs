using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] RotatableMesh m_rotatableMeshPrefab;

    public void LoadLevel()
    {
        GameMode.m_RotatableMeshPrefab = m_rotatableMeshPrefab;
        GetComponent<ChangeScene>().LoadScene("Level");
    }
}

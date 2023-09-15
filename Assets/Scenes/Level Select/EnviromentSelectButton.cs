using UnityEngine;

public class EnviromentSelectButton : MonoBehaviour
{
    [SerializeField] Enviroment m_enviromentPrefab;
    public void EnviromentSelectButtonOnClick() { FindObjectOfType<LevelSelect>().UpdateEnviroment(m_enviromentPrefab); }    
}
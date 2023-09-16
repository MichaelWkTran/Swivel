using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera), typeof(Volume))]
public class Enviroment : MonoBehaviour
{
    public static ObjectGUIDGroup m_guidGroup => (ObjectGUIDGroup)AssetDatabase.LoadAssetAtPath
    (
        "Assets/Enviroments/Enviroments GUID Group.asset",
        typeof(ObjectGUIDGroup)
    );
    static Enviroment m_currentEnviroment = null; //Current enviroment
    public static Enviroment m_CurrentEnviroment
    {
        get
        {
            if (m_currentEnviroment == null) m_currentEnviroment = m_guidGroup.GetComponentByGUID<Enviroment>(SaveSystem.m_data.m_currentEnviromentGUID);
            return m_currentEnviroment;
        }

        set
        {
            m_currentEnviroment = value;

            if (m_currentEnviroment == null) return;
            SaveSystem.m_data.m_currentEnviromentGUID = m_guidGroup.GetGUIDFromObject(m_currentEnviroment.gameObject);
        }
    }

    public Color m_colour; //What colour the shape would be
    public Material m_skyboxMaterial; //What skybox would be shown in the background

    void Start()
    {
        //Copy camera from enviroment to main camera and destroy it from the enviroment
        ComponentUtils.CopyComponent(GetComponent<Camera>(), Camera.main);

        //Set skybox material
        RenderSettings.skybox = m_skyboxMaterial;
    }
}
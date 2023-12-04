using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera), typeof(Volume))]
public class Enviroment : MonoBehaviour
{
    public static ObjectGUIDGroup m_guidGroup => Resources.Load<ObjectGUIDGroup>("Enviroments GUID Group");
    static Enviroment m_currentEnviroment = null; //Current enviroment
    public static Enviroment m_CurrentEnviroment
    {
        get
        {
            if (m_currentEnviroment == null) m_currentEnviroment = m_guidGroup.GetObjectByGUID<GameObject>
                    (SaveSystem.m_Data.m_currentEnviromentGUID)?.GetComponent<Enviroment>();
            return m_currentEnviroment;
        }

        set
        {
            m_currentEnviroment = value;

            if (m_currentEnviroment == null) return;
            SaveSystem.m_Data.m_currentEnviromentGUID = m_guidGroup.GetGUIDFromObject(m_currentEnviroment.gameObject);
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
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera), typeof(Volume))]
public class Enviroment : MonoBehaviour
{
    [ObjectID] public string m_ID; //Enviroment ID
    static GuidObjectGroup m_GuidGroup
    {
        get
        {
            return (GuidObjectGroup)AssetDatabase.LoadAssetAtPath(
                "Assets/Enviroments/Enviroments Guid Group.asset",
                typeof(GuidObjectGroup));
        }
    } //What guid group is this object apart of
#if UNITY_EDITOR
    [SerializeField, HideInInspector] string m_initialID; //Used for checking whether the ID has been changed
#endif
    public Color m_colour; //What colour the shape would be
    public Material m_skyboxMaterial; //What skybox would be shown in the background
    static Enviroment m_currentEnviroment = null; //Current enviroment
    public static Enviroment m_EnviromentPrefab
    {
        get
        {
            //Return current enviroment
            if (m_currentEnviroment != null) return m_currentEnviroment;
            
            //Check whether the saved Guid is valid
            if (Guid.TryParse(SaveSystem.m_data.m_currentEnviromentGUID, out _))
            {
                //Search through the guid group to find this enviroment
                m_currentEnviroment = (Enviroment)Array.Find(m_GuidGroup.m_StoredGuidObjects, i => {
                    return ((GameObject)i).GetComponent<Enviroment>().m_ID == SaveSystem.m_data.m_currentEnviromentGUID; });

                //If the guid belongs to a valid enviroment, then return that enviroment
                if (m_currentEnviroment != null) return m_currentEnviroment;
            }

            //If the guid is not valid, get the default prefab
            m_currentEnviroment = (Enviroment)AssetDatabase.LoadAssetAtPath("Assets/Enviroments/Default Enviroment.prefab", typeof(Enviroment));
            if (m_currentEnviroment == null) Debug.LogError("Default enviroment prefab does not exist");

            //Save the data
            SaveSystem.m_data.m_currentEnviromentGUID = m_currentEnviroment.m_ID;
            return m_currentEnviroment;
        }

        set
        {
            //Prevent the variable from being set to null
            if (value == null) return;

            //Set value and save data
            m_currentEnviroment = value;
            SaveSystem.m_data.m_currentEnviromentGUID = m_currentEnviroment.m_ID;
        }
    }

    void Start()
    {
        //Copy camera from enviroment to main camera and destroy it from the enviroment
        ComponentUtils.CopyComponent(GetComponent<Camera>(), Camera.main);

        //Set skybox material
        RenderSettings.skybox = m_skyboxMaterial;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        //Check Unique UITheme ID
        CheckUniqueID();
    }

    void CheckUniqueID()
    {
        if (m_initialID == m_ID) return;
        m_initialID = m_ID;

        //Search asset guids
        if (Array.Find(m_GuidGroup.m_StoredGuidObjects, storedObject => ((GameObject)storedObject).GetComponent<Enviroment>().m_ID == m_ID) != null)
            CheckUniqueID();
    }

#endif
}
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//NOTE: Implement saving system, getting GUID by object, and custom inspector for preventing multiple instances of an object in a array

[CreateAssetMenu(fileName = "Untitled Object GUID Group", menuName = "Object GUID Group")]
public class ObjectGUIDGroup : ScriptableObject
{
    //A struct pairing a Unity Object reference with a GUID string
    [Serializable] public struct ObjectGUID
    {
        //The Unity object to be referenced
        public UnityEngine.Object m_assetReference;

        //A unique identifier for the object
        [ObjectID] public string m_GUID;
    }

    //An array of ObjectGUID structs to store grouped objects
    [SerializeField] ObjectGUID[] m_objectGUIDs;
    public ObjectGUID[] m_ObjectGUIDs => m_objectGUIDs;

    public UnityEngine.Object GetObjectFromGUID(string _comparedGUID) => Array.Find(m_objectGUIDs, i =>
    {
        return i.m_GUID == _comparedGUID;
    }).m_assetReference;

    public T GetObjectByGUID<T>(string _guid) where T : UnityEngine.Object
    {
        //Attempt to retrieve the object with the given GUID
        T assetReference = (T)GetObjectFromGUID(_guid);

        //If the object is not found, use the first asset reference from m_objectGUIDs
        if (assetReference == null) assetReference = (T)m_objectGUIDs[0].m_assetReference;

        //Return the retrieved or default asset
        return assetReference;
    }

    public T GetComponentByGUID<T>(string _guid) where T : Component
    {
        //Attempt to retrieve the component in the asset with the given GUID
        GameObject assetReference = (GameObject)GetObjectFromGUID(_guid);
        T componentReference = null; if (assetReference != null) componentReference = assetReference.GetComponentInChildren<T>(true);

        //If the component is not found, use the first asset reference from m_objectGUIDs
        if (componentReference == null)
        {
            assetReference = (GameObject)m_objectGUIDs[0].m_assetReference;
            componentReference = assetReference.GetComponentInChildren<T>(true);
        }

        //Return the retrieved or default component reference
        return componentReference;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        CheckUniqueIDs();
    }

    void CheckUniqueIDs()
    {
        HashSet<string> uniqueGUIDs = new HashSet<string>();
        List<int> duplicateIndices = new List<int>();

        // Find duplicate GUIDs and collect their indices.
        for (int i = 0; i < m_objectGUIDs.Length; i++)
        {
            ObjectGUID objectGUID = m_objectGUIDs[i];
            if (!uniqueGUIDs.Add(objectGUID.m_GUID)) duplicateIndices.Add(i);
        }

        // Reset GUIDs for duplicates.
        foreach (int duplicateIndex in duplicateIndices)
        {
            ScriptableObjectIdDrawer.ResetGuid(ref m_objectGUIDs[duplicateIndex].m_GUID, this);
        }
    }
#endif
}

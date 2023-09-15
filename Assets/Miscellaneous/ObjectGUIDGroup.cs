using System;
using System.Collections.Generic;
using UnityEngine;

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

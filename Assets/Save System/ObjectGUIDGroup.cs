using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Object GUID Group", menuName = "Object GUID Group")]
public class ObjectGUIDGroup : ScriptableObject
{
    //An array of ObjectGUID structs to store grouped objects
    [SerializeField] ObjectGUID[] m_objectGUIDs;
    public ObjectGUID[] m_ObjectGUIDs => m_objectGUIDs;

    public UnityEngine.Object GetObjectFromGUID(string _guid) => Array.Find(m_objectGUIDs, i => { return i.m_guid == _guid; })?.m_assetReference;
    public T GetObjectByGUID<T>(string _guid) where T : UnityEngine.Object
    {
        //Attempt to retrieve the object with the given GUID
        T assetReference = (T)GetObjectFromGUID(_guid);

        //If the object is not found, use the first asset reference from m_objectGUIDs
        if (assetReference == null) assetReference = (T)m_objectGUIDs[0].m_assetReference;

        //Return the retrieved or default asset
        return assetReference;
    }
    public string GetGUIDFromObject(UnityEngine.Object _object) => Array.Find(m_objectGUIDs, i => i.m_assetReference == _object).m_guid;

#if UNITY_EDITOR
    void OnValidate()
    {
        CheckUniqueIDs();

        //Ensure all stored objects are unique
        {
            var uniqueAssets = new HashSet<UnityEngine.Object>();
            for (int i = 0; i < m_objectGUIDs.Length; i++)
            {
                if (uniqueAssets.Contains(m_objectGUIDs[i].m_assetReference))
                {
                    Debug.Log("The asset named " + m_objectGUIDs[i].m_assetReference.name + " is already referenced in this group", this);
                    m_objectGUIDs[i].m_assetReference = null;
                }
                else uniqueAssets.Add(m_objectGUIDs[i].m_assetReference);
            }
        }
    }

    void CheckUniqueIDs()
    {
        HashSet<string> uniqueGUIDs = new HashSet<string>();
        List<int> duplicateIndices = new List<int>();

        //Find duplicate GUIDs and collect their indices.
        for (int i = 0; i < m_objectGUIDs.Length; i++)
        {
            ObjectGUID objectGUID = m_objectGUIDs[i];
            if (!uniqueGUIDs.Add(objectGUID.m_guid)) duplicateIndices.Add(i);
        }

        //Reset GUIDs for duplicates.
        foreach (int duplicateIndex in duplicateIndices)
        {
            ScriptableObjectIdDrawer.ResetGuid(ref m_objectGUIDs[duplicateIndex].m_guid, this);
        }
    }
#endif
}
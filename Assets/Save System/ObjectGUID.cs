using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Object GUID", menuName = "Object GUID")]
public class ObjectGUID : ScriptableObject
{
    //The Unity object to be referenced
    public Object m_assetReference;

    //A unique identifier for the object
    [ObjectID] public string m_GUID;
}

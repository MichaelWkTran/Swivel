using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Guid Object Group", menuName = "Guid Object Group")]
public class GuidObjectGroup : ScriptableObject
{
    [SerializeField] Object[] m_storedGuidObjects;
    public Object[] m_StoredGuidObjects { get { return m_storedGuidObjects; } }
}

using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Sprite Group", menuName = "Sprite Group")]
public class SpriteGroup : ScriptableObject
{
    [ObjectID] public string m_ID; //UI ID
#if UNITY_EDITOR
    [SerializeField, HideInInspector] string m_initialID; //Used for checking whether the ID has been changed
    [SerializeField] GuidObjectGroup m_guidGroup; //What guid group is this object apart of
    [SerializeField] uint m_numberOfSprites; //Number of sprites stored in this sprite group
#endif

    public Sprite[] m_sprites; //Stored sprites to match with a silhouette
    public Sprite[] m_silhouetteSprites; //Stored silhouette to match with a sprites

#if UNITY_EDITOR
    public Texture2D m_spritesTexture; //The sprite sheet used to extract the sprites to match
    public Texture2D m_silhouetteSpritesTexture; //The sprite sheet used to extract the silhouette sprites

    void OnValidate()
    {
        //Update the size of sprite arrays
        Array.Resize(ref m_sprites, (int)m_numberOfSprites);
        Array.Resize(ref m_silhouetteSprites, (int)m_numberOfSprites);

        //Check Unique UITheme ID
        CheckUniqueID();
    }

    void CheckUniqueID()
    {
        if (m_guidGroup == null) { Debug.LogError("Guid group is not assigned", this); return; }
        if (m_initialID == m_ID) return;
        m_initialID = m_ID;

        //Search asset guids
        if (Array.Find(m_guidGroup.m_StoredGuidObjects, storedObject => ((SpriteGroup)storedObject).m_ID == m_ID) != null) CheckUniqueID();
    }

#endif

    public Sprite GetSilhouetteFromSprite(Sprite _sprite)
    {
        int index = Array.IndexOf(m_sprites, _sprite);
        if (index < 0) return null;

        return m_silhouetteSprites[index];
    }

    public Sprite GetSpriteFromSilhouette(Sprite _silhouette)
    {
        int index = Array.IndexOf(m_silhouetteSprites, _silhouette);
        if (index < 0) return null;

        return m_sprites[index];
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteGroup))]
public class SpriteGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SpriteGroup spriteGroup = (SpriteGroup)target;

        if (GUILayout.Button("Set Sprites from Texture") && spriteGroup.m_spritesTexture != null)
        {
            Array.Copy
            (
                Resources.LoadAll<Sprite>(spriteGroup.m_spritesTexture.name),
                spriteGroup.m_sprites,
                spriteGroup.m_sprites.Length
            );
        }

        if (GUILayout.Button("Set Silhouette Sprites from Texture") && spriteGroup.m_silhouetteSpritesTexture != null)
        {
            Array.Copy
            (
                Resources.LoadAll<Sprite>(spriteGroup.m_silhouetteSpritesTexture.name),
                spriteGroup.m_silhouetteSprites,
                spriteGroup.m_silhouetteSprites.Length
            );
        }
    }
}
#endif
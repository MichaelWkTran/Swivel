using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Sprite Group", menuName = "Sprite Group")]
public class SpriteGroup : ScriptableObject
{
#if UNITY_EDITOR
    [TextArea] public string m_description;
#endif

    public static ObjectGUIDGroup m_guidGroup => Resources.Load<ObjectGUIDGroup>("Assets/Scenes/Level/Sprite Groups/Sprite GUID Groups.asset"); //What guid group is this object apart of
    static SpriteGroup m_currentSpriteGroup = null; //Current sprite group
    public static SpriteGroup m_CurrentSpriteGroup
    {
        get
        {
            if (m_currentSpriteGroup == null) m_currentSpriteGroup = m_guidGroup.GetObjectByGUID<SpriteGroup>(SaveSystem.m_Data.m_currentSpriteGroupGUID); 
            return m_currentSpriteGroup;
        }

        set
        {
            m_currentSpriteGroup = value;
            SaveSystem.m_Data.m_currentSpriteGroupGUID = m_guidGroup.GetGUIDFromObject(m_currentSpriteGroup);
        }
    }

    public Sprite[] m_sprites; //Stored sprites to match with a silhouette
    public Sprite[] m_silhouetteSprites; //Stored silhouette to match with a sprites

#if UNITY_EDITOR
    [SerializeField] uint m_numberOfSprites;//Number of sprites stored in this sprite group
    public Texture2D m_spritesTexture; //The sprite sheet used to extract the sprites to match
    public Texture2D m_silhouetteSpritesTexture; //The sprite sheet used to extract the silhouette sprites
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
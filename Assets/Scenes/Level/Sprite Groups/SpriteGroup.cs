using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Sprite Group", menuName = "Sprite Group")]
public class SpriteGroup : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField] uint m_numberOfSprites;
#endif

    public Sprite[] m_sprites;
    public Sprite[] m_silhouetteSprites;

#if UNITY_EDITOR
    public Texture2D m_spritesTexture;
    public Texture2D m_silhouetteSpritesTexture;
#endif

    void OnValidate()
    {
        Array.Resize(ref m_sprites, (int)m_numberOfSprites);
        Array.Resize(ref m_silhouetteSprites, (int)m_numberOfSprites);
    }

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
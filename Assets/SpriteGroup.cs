using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Sprite Group", menuName = "Sprite Group", order = 1)]
public class SpriteGroup : ScriptableObject
{
    public uint m_numberOfSprites;
    public Sprite[] m_sprites;
    public Sprite[] m_silhouetteSprites;

    void OnValidate()
    {
        Array.Resize(ref m_sprites, (int)m_numberOfSprites);
        Array.Resize(ref m_silhouetteSprites, (int)m_numberOfSprites);
    }

    public Sprite GetSpriteFromSilhouette(Sprite _sprite)
    {
        return m_silhouetteSprites[Array.IndexOf(m_sprites, _sprite)];
    }

    public Sprite GetSilhouetteFromSprite(Sprite _silhouette)
    {
        return m_sprites[Array.IndexOf(m_silhouetteSprites, _silhouette)];
    }
}
using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

[CreateAssetMenu(fileName = "Untitled Sprite Group", menuName = "Sprite Group")]
public class SpriteGroup : ObjectGUID
{
#if UNITY_EDITOR
    [TextArea] public string m_description;
#endif

    public static ObjectGUIDGroup m_guidGroup => Resources.Load<ObjectGUIDGroup>("Sprite GUID Groups"); //What guid group is this object apart of
    static SpriteGroup m_currentSpriteGroup = null; //Current sprite group
    public static SpriteGroup m_CurrentSpriteGroup
    {
        get
        {
            if (m_currentSpriteGroup == null) m_currentSpriteGroup = m_guidGroup.GetObjectByGUID<SpriteGroup>
                    (SaveSystem.m_Data.m_currentSpriteGroupGUID);
            return m_currentSpriteGroup;
        }

        set
        {
            m_currentSpriteGroup = value;
            SaveSystem.m_Data.m_currentSpriteGroupGUID = m_guidGroup.GetGUIDFromObject(m_currentSpriteGroup);
        }
    }

    public Sprite[] m_sprites; //Stored silhouette sprites
#if UNITY_EDITOR
    public Texture2D m_spritesTexture; //The sprite sheet used to extract the sprites to match
#endif

    void OnValidate()
    {
        m_assetReference = this;
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
            List<UnityEngine.Object> spriteList = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(spriteGroup.m_spritesTexture)).ToList(); spriteList.RemoveAt(0);
            spriteGroup.m_sprites = new Sprite[spriteList.Count];

            Array.Copy
            (
                spriteList.ToArray(),
                spriteGroup.m_sprites,
                spriteGroup.m_sprites.Length
            );
        }
    }
}
#endif
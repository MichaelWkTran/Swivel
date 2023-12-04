using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ImageAnimationLoop : MonoBehaviour
{
    public Image m_image;
    public Sprite[] m_animationFrames;
    public float m_frameDuration = 0.2f;
#if UNITY_EDITOR
    public Texture2D m_spritesTexture; //The sprite sheet used to extract the sprites
#endif

    void OnEnable()
    {
        StartCoroutine(AnimateLoop());
    }

    private IEnumerator AnimateLoop()
    {
        int frameIndex = 0;

        while (true)
        {
            m_image.sprite = m_animationFrames[frameIndex];
            
            frameIndex = (frameIndex + 1) % m_animationFrames.Length;
            yield return new WaitForSeconds(m_frameDuration);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ImageAnimationLoop))]
public class ImageAnimationLoopEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ImageAnimationLoop imageAnimationLoop = (ImageAnimationLoop)target;

        if (GUILayout.Button("Set Sprites from Texture") && imageAnimationLoop.m_spritesTexture != null)
        {
            List<UnityEngine.Object> spriteList = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(imageAnimationLoop.m_spritesTexture)).ToList(); spriteList.RemoveAt(0);
            imageAnimationLoop.m_animationFrames = new Sprite[spriteList.Count];

            Array.Copy
            (
                spriteList.ToArray(),
                imageAnimationLoop.m_animationFrames,
                imageAnimationLoop.m_animationFrames.Length
            );
        }
    }
}
#endif

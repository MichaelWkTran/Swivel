using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled UI Theme", menuName = "UI Theme")]
public class UIThemes : ScriptableObject
{
    public TMPro.TMP_FontAsset m_font; //The font of the UI
    public float m_fontScale; //The scale of the font to ensure that it is readable with this font
    public Sprite m_currentImageBorder; //The border that displays the image that the player had to match
    public Gradient m_timerBarGradient; //Set the gradient of the timer bar
}

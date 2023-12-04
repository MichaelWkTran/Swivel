using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimationLoop : MonoBehaviour
{
    public Image m_image;
    public Sprite[] m_animationFrames;
    public float m_frameDuration = 0.2f;

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

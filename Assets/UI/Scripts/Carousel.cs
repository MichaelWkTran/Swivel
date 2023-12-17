using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    public ScrollRect m_scrollRect;
    public AnimationCurve m_contentsScale = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
    public float m_contentsScaleFactor = 1.0f;

    void Update()
    {
        RectTransform scrollRectTransform = m_scrollRect.GetComponent<RectTransform>();
        foreach (RectTransform child in m_scrollRect.content)
            child.localScale = Vector3.one * m_contentsScale.Evaluate
                (Vector2.Distance(scrollRectTransform.TransformPoint(scrollRectTransform.rect.center), child.TransformPoint(child.rect.center)) / m_contentsScaleFactor);
    }

    void OnValidate()
    {
        if (!m_scrollRect) m_scrollRect = GetComponent<ScrollRect>();
    }
}

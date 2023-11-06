using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    static ToggleGroup shopSlotToggleGroup = null;

    bool m_isOpen = false;
    [SerializeField] float m_openCloseTime;
    [SerializeField] RectTransform m_uiGroup;
    [SerializeField] Toggle m_showPurchaseToggle;
    [SerializeField] Button m_purchaseButton;

    void Awake()
    {
        if (shopSlotToggleGroup == null)
        {
            shopSlotToggleGroup = new GameObject("Shop Slot Toggle Group").AddComponent<ToggleGroup>();
            shopSlotToggleGroup.allowSwitchOff = true;
        }

        m_showPurchaseToggle.group = shopSlotToggleGroup;
    }

    public void ShowPurchaseToggle()
    {
        if (LeanTween.isTweening(m_uiGroup.gameObject)) return;
        m_isOpen = !m_isOpen;
        
        if (m_isOpen)
        {
            LeanTween.value
            (
                m_uiGroup.gameObject,
                m_uiGroup.offsetMax.x, -m_purchaseButton.GetComponent<RectTransform>().rect.width,
                m_openCloseTime
            )
            .setEaseInCirc()
            .setOnUpdate((float _right) => { m_uiGroup.offsetMax = new Vector2(_right, m_uiGroup.offsetMax.y); });
        }
        else
        {
            LeanTween.value
            (
                m_uiGroup.gameObject,
                m_uiGroup.offsetMax.x, 0.0f,
                m_openCloseTime
            )
            .setEaseInCirc()
            .setOnUpdate((float _right) => { m_uiGroup.offsetMax = new Vector2(_right, m_uiGroup.offsetMax.y); });
        }
    }
}
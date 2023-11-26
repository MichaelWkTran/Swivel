using System;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    enum ItemType { Silouettes, Backgrounds, UIThemes }
    [SerializeField] string m_itemObjectGUID;
    [SerializeField] ItemType m_itemObjectGUIDType;

    bool m_isItemPurchased = false;
    bool m_isOpen = false;
    [SerializeField] float m_openCloseTime;
    [SerializeField] RectTransform m_uiGroup;
    [SerializeField] Toggle m_showPurchaseToggle;
    [SerializeField] Button m_purchaseButton;
    
    EventSystem m_eventSystem;
    Selectable[] m_selectables;

    void Start()
    {
        //Initiate objects references
        m_eventSystem = FindObjectOfType<EventSystem>();
        m_selectables = GetComponentsInChildren<Selectable>();
    }

    void Update()
    {
        //Dont update if the object is purchased
        if (m_isItemPurchased) return;

        //If a selectable from the slot is currently selected, show the purchase button; otherwise, hide it
        if (Array.Exists(m_selectables, i => m_eventSystem.currentSelectedGameObject == i.gameObject)) { if (!m_isOpen) ShowPurchaseButton(); }
        else { if (m_isOpen) ShowPurchaseButton(); }
    }

    void ShowPurchaseButton()
    {
        //If a tween is already running on the UI group, return
        if (LeanTween.isTweening(m_uiGroup.gameObject)) return;

        //Set current state of the shop slot
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

    public void PurchaseItem()
    {
        //Set the item as purchased
        m_isItemPurchased = true;

        //If the purchase toggle is open, hide it
        if (m_isOpen) ShowPurchaseButton();

        //Save Purchased Item
        switch(m_itemObjectGUIDType)
        {
            case ItemType.Silouettes:
                {
                    ObjectGUID[] objectGUIDs = SpriteGroup.m_guidGroup?.m_ObjectGUIDs;
                    if (objectGUIDs != null && Array.Exists(objectGUIDs, i => i.m_guid == m_itemObjectGUID))
                    {
                        SaveSystem.m_Data.m_unlockedSpriteGroupGUIDs.Add(m_itemObjectGUID);
                        SelectItem();
                    }
                }
                break;
            
            case ItemType.Backgrounds:
                {
                    ObjectGUID[] objectGUIDs = Enviroment.m_guidGroup?.m_ObjectGUIDs;
                    if (objectGUIDs != null && Array.Exists(objectGUIDs, i => i.m_guid == m_itemObjectGUID))
                    {
                        SaveSystem.m_Data.m_unlockedEnviromentGUIDs.Add(m_itemObjectGUID);
                        SelectItem();
                    }
                }
                break;
            
            case ItemType.UIThemes:
                {
                    ObjectGUID[] objectGUIDs = GameUI.m_guidGroup?.m_ObjectGUIDs;
                    if (objectGUIDs != null && Array.Exists(objectGUIDs, i => i.m_guid == m_itemObjectGUID))
                    {
                        SaveSystem.m_Data.m_unlockedUIThemeGUIDs.Add(m_itemObjectGUID);
                        SelectItem();
                    }
                }
                break;
        }
    }

    public void SelectItem()
    {
        switch (m_itemObjectGUIDType)
        {
            case ItemType.Silouettes:
                SpriteGroup.m_CurrentSpriteGroup = SpriteGroup.m_guidGroup?.GetObjectByGUID<SpriteGroup>(m_itemObjectGUID);
                break;

            case ItemType.Backgrounds:
                Enviroment.m_CurrentEnviroment = Enviroment.m_guidGroup?.GetObjectByGUID<Enviroment>(m_itemObjectGUID);
                break;

            case ItemType.UIThemes:
                GameUI.m_CurrentGameUI = Enviroment.m_guidGroup?.GetObjectByGUID<GameUI>(m_itemObjectGUID);
                FindObjectOfType<LevelSelect>().UpdateEnviroment(null);
                break;
        }
    }
}
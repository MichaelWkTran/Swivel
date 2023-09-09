using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [ObjectID] public string m_ID; //UI ID
#if UNITY_EDITOR
    [SerializeField, HideInInspector] string m_initialID; //Used for checking whether the ID has been changed
    [SerializeField] GuidObjectGroup m_guidGroup; //What guid group is this object apart of
#endif
    public Canvas m_mainCanvas; //The canvas that shows the main UI
    public Canvas m_gameOverCanvas; //The canvas that shows the game over screen
    public Canvas m_pauseCanvas; //The canvas that shows the pause screen
    public Image m_currentImage; //The image that shows what sprite on the rotatable mesh that the player has to match
    public TMPro.TMP_Text m_scoreValueText; //The text that displays the current score of the game
    public Slider m_timerBar; //The UI that shows how much time is left
    public Gradient m_timerBarGradient; //The colour of the timer bar depending on how much time is left

    GameMode m_gameMode;

    void Start()
    {
        m_gameMode = FindObjectOfType<GameMode>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        //Check Unique UITheme ID
        CheckUniqueID();
    }

    void CheckUniqueID()
    {
        if (m_guidGroup == null) { Debug.LogError("Guid group is not assigned", this); return; }
        if (m_initialID == m_ID) return;
        m_initialID = m_ID;

        //Search asset guids
        if (Array.Find(m_guidGroup.m_StoredGuidObjects, storedObject => ((GameObject)storedObject).GetComponent<GameUI>().m_ID == m_ID) != null)
            CheckUniqueID();
    }

#endif

    public void OnPauseButtonClick()
    {
        m_gameMode.Pause();
    }

    public void OnResumeButtonClick()
    {
        m_gameMode.UnPause();
    }

    public void OnShareButtonClick()
    {
        m_gameMode.ShareHighScore();
    }
}

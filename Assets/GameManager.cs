using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float m_score; //The 
    public uint m_round { get; private set; } = 0;
    [SerializeField] float m_startTime;
    [SerializeField] float m_timeDecreaseRate;
    [SerializeField] float m_minTime; //The fastest time a round can have

    [Header("UI")]
    [SerializeField] Slider m_timerBar; //The UI that shows how much time is left

    void Start()
    {
        //Setup timer
        m_timerBar.value = m_timerBar.maxValue = m_startTime;
    }

    void Update()
    {
        //Update Timer
        m_timerBar.value -= Time.deltaTime;
    }

    public void A()
    {
        //Add Score
        m_score += m_timerBar.value / m_timerBar.maxValue;

        //Reset Timer
        m_timerBar.maxValue -= m_timeDecreaseRate;
        if (m_timerBar.maxValue < m_minTime) m_timerBar.maxValue = m_minTime;
        m_timerBar.value = m_timerBar.maxValue;
    }
}

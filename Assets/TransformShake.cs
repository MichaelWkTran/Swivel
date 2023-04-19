using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformShake : MonoBehaviour
{
    public float m_shakeAmplitude;
    Vector3 m_originalPos;

    void Start()
    {
        m_originalPos = transform.position;
    }

    private void OnDisable()
    {
        transform.position = m_originalPos;
    }

    void Update()
    {
        if (m_shakeAmplitude <= 0) return;
        transform.position = m_originalPos + ((Vector3)Random.insideUnitCircle * Random.value * m_shakeAmplitude);
    }
}

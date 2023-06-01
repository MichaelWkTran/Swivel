using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatableMeshGameOver : MonoBehaviour
{
    [SerializeField] float m_explosionForce;
    [SerializeField] float m_explosionRadius;

    void Start()
    {
        foreach(var pieces in GetComponentsInChildren<Rigidbody>())
            pieces.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, 0.0f, ForceMode.Impulse);
    }
}

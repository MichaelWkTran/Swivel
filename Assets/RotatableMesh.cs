using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RotatableMesh : MonoBehaviour
{
    [SerializeField] float m_dragSensitivity = 1.0f;
    [SerializeField] float m_snapSlerpFactor = 1.0f;
    public bool m_isDragging = false;

    [Header("Components")]
    [SerializeField] MeshFilter m_meshFilter;
    [SerializeField] Collider m_collider;

    Vector2 m_currentMousePos;
    Vector2 m_prevMousePos;
    int m_targetNormalIndex = -1;
    [SerializeField] Vector3[] m_normals;
    
    void Start()
    {
        List<Vector3> normals = new List<Vector3>();
        foreach (Vector3 normal in m_meshFilter.mesh.normals)
        {
            if (normals.Find(i => i == normal) != Vector3.zero) continue;
            normals.Add(normal);
        }

        m_normals = normals.ToArray();
    }

    void Update()
    {
        //Rotate the mesh with the mouse
        if (IsMouseHeld())
        {
            //Rotate the mesh
            Vector2 difference = m_currentMousePos - m_prevMousePos;
            difference *= m_dragSensitivity;
            transform.RotateAround(transform.position, Camera.main.transform.up, -difference.x);
            transform.RotateAround(transform.position, Camera.main.transform.right, difference.y);

            //Update mouse previous position
            m_prevMousePos = m_currentMousePos;
        }
        //Rotate the mesh to allign with the camera
        else
        {
            //Update current and previous mouse position so that they dont cause the mesh to snap when rotating
            m_currentMousePos = m_prevMousePos = Input.mousePosition;

            if (m_isDragging || m_targetNormalIndex < 0) m_targetNormalIndex = GetClosestNormalIndex();
            transform.rotation = Quaternion.Slerp
            (
                transform.rotation, 
                Quaternion.LookRotation(m_normals[m_targetNormalIndex]) * Quaternion.LookRotation(-Camera.main.transform.forward), m_snapSlerpFactor * Time.deltaTime);


            m_isDragging = false;
        }
    }

    bool IsMouseHeld()
    {
        //Check whether mouse is pressed
        bool isMouseHeld = Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2);
        if (isMouseHeld)
        {
            m_currentMousePos = Input.mousePosition;
            return isMouseHeld;
        }

        //Check whether the player is touching
        isMouseHeld = Input.touchCount > 0;
        if (isMouseHeld)
        {
            m_currentMousePos = Input.GetTouch(0).position;
            return isMouseHeld;
        }

        return isMouseHeld;
    }

    int GetClosestNormalIndex()
    {
        //Get the angle between the normals of the mesh in local space, and the camera view direction
        float GetNormalAngle(int _index)
        {
            return Vector3.Angle(-Camera.main.transform.forward, transform.rotation * m_normals[_index]);
        }

        //Loop through all normals to see which normal is closest to the camera view direction
        int selectedNormalIndex = 0;
        float selectedNormalAngle = GetNormalAngle(selectedNormalIndex);
        
        for (int i = 1; i < m_normals.Length; i++)
        {
            float normalAngle = GetNormalAngle(i);
            if (normalAngle >= selectedNormalAngle) continue;

            selectedNormalIndex = i;
            selectedNormalAngle = normalAngle;
        }

        return selectedNormalIndex;
    }
}

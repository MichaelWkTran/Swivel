using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RotatableMesh : MonoBehaviour
{
    [SerializeField] float m_dragSensitivity = 1.0f;
    [SerializeField] float m_snapSlerpFactor = 1.0f;
    public bool m_isDragging = false;

    [Header("Rendering")]
    [SerializeField] Color m_faceColour;
    [SerializeField] Material m_material;
    [SerializeField] Texture2D[] m_textures = new Texture2D[0];

    [Header("Components")]
    [SerializeField] MeshFilter m_meshFilter;
    [SerializeField] MeshRenderer m_meshRenderer;

    Vector2 m_currentMousePos;
    Vector2 m_prevMousePos;
    [SerializeField] int m_targetNormalIndex = -1;
    [SerializeField] Vector3[] m_normals;

    public void UpdateMeshMaterials()
    {
        //Get unique normals in mesh
        if (m_meshFilter != null)
        {
            List<Vector3> normals = new List<Vector3>();
            foreach (Vector3 normal in m_meshFilter.mesh.normals)
            {
                if (normals.Find(i => i == normal) != Vector3.zero) continue;
                normals.Add(normal);
            }
            m_normals = normals.ToArray();

            //Ensure that the size of the texture matches that of the number of normals in the mesh
            if (m_textures != null)
            {
                if (m_textures.Length != m_normals.Length)
                {
                    Texture2D[] newTextures = new Texture2D[m_normals.Length];
                    for (int i = 0; i < Mathf.Min(m_textures.Length, newTextures.Length); i++) newTextures[i] = m_textures[i];
                    m_textures = newTextures;
                }
            }
        }

        //Set materials of mesh
        if (m_material == null || m_meshRenderer == null) return;

        m_material.SetColor("_FaceColour", m_faceColour);
        Material[] materials = new Material[m_normals.Length];
        for (int i = 0; i < m_normals.Length; i++) materials[i] = m_material;
        m_meshRenderer.sharedMaterials = materials;

        for (int i = 0; i < m_normals.Length && m_meshRenderer != null; i++)
        {
            m_meshRenderer.materials[i].SetTexture("_Texture2D", m_textures[i]);
        }
    }

    void Update()
    {
        //Rotate the mesh with the mouse
        if (IsMouseHeld())
        {
            m_isDragging = true;

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
                Quaternion.LookRotation(m_normals[m_targetNormalIndex]) * Quaternion.LookRotation(-Camera.main.transform.forward), m_snapSlerpFactor * Time.deltaTime
            );


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
            //Vector3 a = transform.TransformPoint(m_normals[_index]);
            //a = Camera.main.transform.position - a;

            //float angle = Vector3.Angle(-Camera.main.transform.forward, transform.rotation * m_normals[_index]);
            //return Vector3.Distance(a, Camera.main.transform.position);
            return Vector3.Angle(-Camera.main.transform.forward, transform.TransformDirection(m_normals[_index]));
            //return (transform.TransformPoint(m_normals[_index]) - Camera.main.transform.position).sqrMagnitude;
        }

        //Loop through all normals to see which normal is closest to the camera view direction
        int selectedNormalIndex = 0;
        float selectedNormalAngle = float.PositiveInfinity;// = GetNormalAngle(selectedNormalIndex);
        
        for (int i = 0; i < m_normals.Length; i++)
        {
            float normalAngle = GetNormalAngle(i);
            if (normalAngle >= selectedNormalAngle) continue;

            selectedNormalIndex = i;
            selectedNormalAngle = normalAngle;
        }

        return selectedNormalIndex;
    }
}

[CustomEditor(typeof(RotatableMesh))]
public class RotatableMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RotatableMesh myScript = (RotatableMesh)target;
        if (GUILayout.Button("Build Object"))
        {
            myScript.UpdateMeshMaterials();
        }
    }
}
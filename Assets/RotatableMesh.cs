using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof (MeshRenderer))]
public class RotatableMesh : MonoBehaviour
{
    [SerializeField] float m_dragSensitivity = 1.0f;
    [SerializeField] float m_snapSlerpFactor = 1.0f;
    public bool m_isDragging = false;
    [SerializeField] Color m_faceColour;

    Vector2 m_currentMousePos;
    Vector2 m_prevMousePos;
    int m_targetNormalIndex = -1;
    SpriteRenderer[] m_faceTextures;

    void Start()
    {
        m_faceTextures = new SpriteRenderer[transform.childCount];
        for (int i = 0; i < m_faceTextures.Length; i++) m_faceTextures[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
    }

    void OnValidate()
    {
        GetComponent<MeshRenderer>().sharedMaterial.color = m_faceColour;
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

            //
            if (m_isDragging || m_targetNormalIndex < 0) m_targetNormalIndex = GetClosestNormalIndex();

            //
            transform.rotation = Quaternion.Slerp
            (
                m_faceTextures[m_targetNormalIndex].transform.rotation,
                Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up),
                m_snapSlerpFactor * Time.deltaTime
            ) * Quaternion.Inverse(m_faceTextures[m_targetNormalIndex].transform.localRotation);

            //
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
            return (m_faceTextures[_index].transform.position - Camera.main.transform.position).sqrMagnitude;
        }

        //Loop through all normals to see which normal is closest to the camera view direction
        int selectedNormalIndex = 0;
        float selectedNormalAngle = float.PositiveInfinity;// = GetNormalAngle(selectedNormalIndex);

        for (int i = 0; i < m_faceTextures.Length; i++)
        {
            float normalAngle = GetNormalAngle(i);
            if (normalAngle >= selectedNormalAngle) continue;

            selectedNormalIndex = i;
            selectedNormalAngle = normalAngle;
        }

        return selectedNormalIndex;
    }
}

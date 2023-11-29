using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(MeshRenderer))]
public class RotatableMesh : MonoBehaviour
{
    public static float m_dragSensitivity = 1.5f;
    static float m_snapSlerpFactor = 5.0f;
    public bool m_isDragging = false;

    Vector2 m_currentMousePos;
    Vector2 m_prevMousePos;
    public int m_targetFaceIndex { get; private set; } = -1;
    
    public SpriteRenderer[] m_faceTextures { get; private set; }
    public float m_faceTextureOffset = 0.01f;

    //Rendering variables
    public static Color m_shapeColour;
    float m_currentEmissiveIntensity;
    public float m_emissiveIntensity;
    Material m_material;

    //Game Over Variables
    [SerializeField] RotatableMeshGameOver m_rotatableMeshGameOverPrefab;
    public RotatableMeshGameOver m_RotatableMeshGameOverPrefab { get { return m_rotatableMeshGameOverPrefab; } }

    //Components
    public ParticleSystem m_winParticles;

    void Start()
    {
        //Set Shape Colour
        m_material = GetComponent<MeshRenderer>().sharedMaterial;
        m_material.color = m_shapeColour;//Enviroment.m_CurrentEnviroment.m_colour;

        //Select Random Image from m_SpriteGroupAsset
        var avalibleSprites = new List<Sprite>(SpriteGroup.m_CurrentSpriteGroup.m_sprites);
        m_faceTextures = GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < m_faceTextures.Length; i++)
        {
            int spriteIndex = Random.Range(0, avalibleSprites.Count);
            m_faceTextures[i].sprite = avalibleSprites[spriteIndex];
            avalibleSprites.RemoveAt(spriteIndex);
        }

        //For debugging, select sequencial image from m_SpriteGroupAsset
        //for (int i = 0; i < m_faceTextures.Length; i++) m_faceTextures[i].sprite = avalibleSprites[i];
    }

    void Update()
    {
        m_material.SetColor("_EmissionColor", m_material.color * m_emissiveIntensity);
        
        //Dont execute if the game is paused
        if (Time.deltaTime <= 0.0f) return;

        //Rotate the mesh with the mouse
        if (IsMouseHeld())
        {
            //Rotate the mesh
            Vector2 difference = m_isDragging ? m_currentMousePos - m_prevMousePos : Vector2.zero;
            difference *= m_dragSensitivity;
            transform.RotateAround(transform.position, Camera.main.transform.up, -difference.x);
            transform.RotateAround(transform.position, Camera.main.transform.right, difference.y);

            //Set Dragging
            m_isDragging = true;

            //Update mouse previous position
            m_prevMousePos = m_currentMousePos;
        }
        //Rotate the mesh to allign with the camera
        else
        {
            //Get the index of the target face 
            if (m_isDragging || m_targetFaceIndex < 0)
            {
                m_targetFaceIndex = GetClosestFaceIndex();
                FindObjectOfType<GameMode>().WinRound();
            }

            //Rotate the cube towards to the target face
            transform.rotation = Quaternion.Slerp
            (
                m_faceTextures[m_targetFaceIndex].transform.rotation,
                Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up),
                m_snapSlerpFactor * Time.deltaTime
            ) * Quaternion.Inverse(m_faceTextures[m_targetFaceIndex].transform.localRotation);

            //Reset dragging
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

    int GetClosestFaceIndex()
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

    public SpriteRenderer GetCurrentFace()
    {
        return m_faceTextures[m_targetFaceIndex];
    }

    public void FractureShape()
    {
        Instantiate(m_rotatableMeshGameOverPrefab, transform.position, Quaternion.identity);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RotatableMesh))]
public class RotatableMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RotatableMesh myScript = (RotatableMesh)target;
        if (GUILayout.Button("Set Sprite Transforms")) SetSpriteTransforms(myScript);
    }

    class Face
    {
        public List<Vector3> m_vecticies = new List<Vector3>();
        public Vector3 m_normal;
        public Vector4 m_tangent;

        public Vector3 GetCentrePos()
        {
            Vector3 centrePos = Vector3.zero;
            foreach (Vector3 vertex in m_vecticies) centrePos += vertex;
            centrePos /= m_vecticies.Count;

            return centrePos;
        }
    }

    void SetSpriteTransforms(RotatableMesh _rotatableMesh)
    {
        //Get Normals and Positions
        Mesh mesh = _rotatableMesh.GetComponent<MeshFilter>().sharedMesh;
        List<Face> faces = new List<Face>();

        foreach (int triangle in mesh.triangles)
        {
            //Check whether triangle belongs to existing face exists based on normal
            Face face = faces.Find(i => i.m_normal == mesh.normals[triangle]);

            //If no face exist, then add a new face
            if (face == null)
            {
                face = new Face(); faces.Add(face);
                face.m_normal = mesh.normals[triangle];
                face.m_tangent = mesh.tangents[triangle];
            }

            //Add Verticies to face
            face.m_vecticies.Add(mesh.vertices[triangle]);
        }

        //Get all sprite faces
        var faceTextures = new SpriteRenderer[_rotatableMesh.transform.childCount];
        for (int i = 0; i < faceTextures.Length; i++) faceTextures[i] = _rotatableMesh.transform.GetChild(i).GetComponent<SpriteRenderer>();

        //Set their positions
        for (int i = 0; i < Mathf.Min(faceTextures.Length, faces.Count) ; i++)
        {
            faceTextures[i].transform.localPosition = faces[i].GetCentrePos() + (faces[i].m_normal * _rotatableMesh.m_faceTextureOffset);
            faceTextures[i].transform.localRotation = Quaternion.LookRotation(-faces[i].m_normal, Vector3.up);
        }
    }
}
#endif
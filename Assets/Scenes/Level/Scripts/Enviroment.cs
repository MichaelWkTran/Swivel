using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera), typeof(Volume))]
public class Enviroment : MonoBehaviour
{
    public Color m_colour;
    public Material m_skyboxMaterial;
    
    void Start()
    {
        //Copy camera from enviroment to main camera and destroy it from the enviroment
        ComponentUtils.CopyComponent(GetComponent<Camera>(), Camera.main);

        //Set skybox material
        RenderSettings.skybox = m_skyboxMaterial;
    }
}

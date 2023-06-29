using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera), typeof(Volume))]
public class Enviroment : MonoBehaviour
{
    public Color m_colour;
    public Material m_skyboxMaterial;
    
    public void Start()
    {
        //Destroy current camera
        GameObject mainCameraGameObject = Camera.main.gameObject;

        //Copy camera from enviroment to main camera and destroy it from the enviroment
        Camera enviromentCamera = GetComponent<Camera>();
        ComponentUtils.CopyComponent(enviromentCamera, mainCameraGameObject);

        //Set skybox material
        RenderSettings.skybox = m_skyboxMaterial;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Camera))]
public class Enviroment : MonoBehaviour
{
    public Color m_colour;

    public void SetEnviromentAndCamera()
    {
        //Destroy current camera
        GameObject mainCameraGameObject = Camera.main.gameObject;

        //Copy camera from enviroment to main camera and destroy it from the enviroment
        Camera enviromentCamera = GetComponent<Camera>();
        ComponentUtils.CopyComponent(enviromentCamera, mainCameraGameObject);
        Destroy(enviromentCamera.GetComponent<HDAdditionalCameraData>());
    }
}

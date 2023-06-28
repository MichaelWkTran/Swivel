using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera), typeof(Volume))]
public class Enviroment : MonoBehaviour
{
    public Color m_colour;
    public float m_bloomIntensity;
    
    public void SetEnviromentAndCamera()
    {
        //Destroy current camera
        GameObject mainCameraGameObject = Camera.main.gameObject;

        //Copy camera from enviroment to main camera and destroy it from the enviroment
        Camera enviromentCamera = GetComponent<Camera>();
        ComponentUtils.CopyComponent(enviromentCamera, mainCameraGameObject);
    }

    public void ResetBloomIntensity()
    {
        Volume volume = GetComponent<Volume>();
        Bloom bloom; if (!volume.profile.TryGet(out bloom)) return;

        bloom.intensity.value = m_bloomIntensity;
    }
}

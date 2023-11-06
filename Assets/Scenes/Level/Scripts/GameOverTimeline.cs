using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Playables;

public class GameOverTimeline : MonoBehaviour
{
    public float m_bloomIntensity;
    public bool m_animatingBloom;

    public float m_emissionIntensity;
    public bool m_animatingEmission;

    Enviroment m_enviroment;
    RotatableMesh m_rotatableMesh;
    [SerializeField] ParticleSystem m_deathParticles;

#if UNITY_EDITOR
    void OnValidate()
    {
        enabled = false;
    }
#endif

    void Start()
    {
        //Get Refrences
        m_enviroment = FindObjectOfType<Enviroment>();
        m_rotatableMesh = FindObjectOfType<RotatableMesh>();

        //Set the colour of the death particles
        var deathParticlesMain = m_deathParticles.main;
        deathParticlesMain.startColor = m_enviroment.m_colour;

        //Setup binding in director
        var director = GetComponent<PlayableDirector>();
        director.playableAsset = Resources.Load<PlayableAsset>("Assets/Scenes/Level/Game Over.playable");
        foreach (var playableAssetOutput in director.playableAsset.outputs) switch (playableAssetOutput.streamName)
            {
                case "Rotatable Mesh Animation":
                    director.SetGenericBinding(playableAssetOutput.sourceObject, m_rotatableMesh.GetComponent<Animator>());
                    break;

                case "Rotatable Mesh Activation":
                    director.SetGenericBinding(playableAssetOutput.sourceObject, m_rotatableMesh.gameObject);
                    break;
            }

        //Play director
        director.Play();
    }

    void Update()
    {
        AnimateBloom();
        AnimateEmission();
    }

    void AnimateBloom()
    {
        if (!m_animatingBloom) return;
        Volume volume = m_enviroment.GetComponent<Volume>();
        Bloom bloom; if (!volume.profile.TryGet(out bloom)) return;
        
        bloom.intensity.value = m_bloomIntensity;
    }

    void AnimateEmission()
    {
        if (!m_animatingEmission) return;

        MeshRenderer meshRenderer = m_rotatableMesh.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.SetColor("_EmissionColor", meshRenderer.sharedMaterial.color * m_emissionIntensity);
    }

    public void FractureShape()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        m_rotatableMesh.FractureShape();
    }
}

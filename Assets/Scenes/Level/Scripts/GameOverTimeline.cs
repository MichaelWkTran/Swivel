using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;
using System;
using System.Collections;

public class GameOverTimeline : MonoBehaviour
{

    public float m_bloomIntensity;
    public bool m_animatingBloom;

    public float m_emissionIntensity;
    public bool m_animatingEmission;

    [SerializeField] PlayableAsset m_timeline;
    [SerializeField] ParticleSystem m_deathParticles;
    Enviroment m_enviroment;
    RotatableMesh m_rotatableMesh;
    CanvasGroup m_gameOverCanvasGroup;

    bool m_isGameOverOpen = false;

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
        director.playableAsset = m_timeline;
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

    void OnEnable()
    {
        m_gameOverCanvasGroup = FindObjectOfType<GameUI>().m_gameOverCanvas.GetComponent<CanvasGroup>();
        m_gameOverCanvasGroup.alpha = 0.0f;
        m_gameOverCanvasGroup.interactable = false;
    }

    void Update()
    {
        AnimateBloom();
        AnimateEmission();


        //Open game over Screen via input
        if (!m_isGameOverOpen)
        {
            bool isPointerDown = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) ||
                Array.Exists(Input.touches, i => i.phase == TouchPhase.Began);

            if (isPointerDown) ShowGameOverScreen();
        }
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

    public void ShowGameOverScreen()
    {
        if (LeanTween.isTweening(m_gameOverCanvasGroup.gameObject) || m_isGameOverOpen) return;
        IEnumerator SetGameOverInteractable()
        {
            yield return new WaitForSeconds(0.1f);
            m_gameOverCanvasGroup.interactable = true;
        }

        LeanTween.alphaCanvas(m_gameOverCanvasGroup, 1.0f, 0.5f).setEaseLinear();
        m_isGameOverOpen = true;
        StartCoroutine(SetGameOverInteractable());
    }
}

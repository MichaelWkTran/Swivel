using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] UnityEngine.Audio.AudioMixer m_audioMixer;
    [SerializeField] Slider m_sfxVolumeSlider;
    [SerializeField] Slider m_musicVolumeSlider;
    [SerializeField] TMPro.TMP_Dropdown m_graphicsDropdown;
    [SerializeField] Slider m_dragSensitivitySlider;

    void Start()
    {
        {
            float sfxVolumeValue; m_audioMixer.GetFloat("sfxVolume", out sfxVolumeValue);
            m_sfxVolumeSlider.value = sfxVolumeValue;
        }

        {
            float musicVolumeValue; m_audioMixer.GetFloat("musicVolume", out musicVolumeValue);
            m_musicVolumeSlider.value = musicVolumeValue;
        }

        {
            m_dragSensitivitySlider.value = RotatableMesh.m_dragSensitivity;
        }

        m_graphicsDropdown.value = QualitySettings.GetQualityLevel();
    }

    public void SetSFXVolume(float _value)
    {
        m_audioMixer.SetFloat("sfxVolume", SaveSystem.m_Data.m_sfxVolume = _value);
    }

    public void SetMusicVolume(float _value)
    {
        m_audioMixer.SetFloat("musicVolume", SaveSystem.m_Data.m_musicVolume = _value);
    }

    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(SaveSystem.m_Data.m_qualityLevel = _qualityIndex);
    }

    public void SetDragSensitivity(float _value)
    {
        RotatableMesh.m_dragSensitivity = _value;
    }
}

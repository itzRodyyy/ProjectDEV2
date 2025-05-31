using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string volumeParameter;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip clip;
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [Range(0, 1)] [SerializeField] float volume; 
    [SerializeField] float multiplier = 30f;
    private void Awake()
    {
        slider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        if (value <= 0.0001f)
        {
            mixer.SetFloat(volumeParameter, -80f);
        }
        else
        {
            mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
        }

        if (value > 0)
        {
            aud.PlayOneShot(clip, volume);
        }
    }

    private void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(volumeParameter, slider.value);
        slider.value = savedValue;

        if (savedValue <= 0.0001f)
        {
            mixer.SetFloat(volumeParameter, -80f);
        }
        else
        {
            mixer.SetFloat(volumeParameter, Mathf.Log10(savedValue) * multiplier);
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }
}
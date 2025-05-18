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
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiplier);
        aud.PlayOneShot(clip, volume);
    }

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
        mixer.SetFloat(volumeParameter, Mathf.Log10(slider.value) * multiplier);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }
}
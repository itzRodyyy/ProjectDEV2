using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class volumeControl : MonoBehaviour
{
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [SerializeField] float volumeMultiplier = 30f;
    private void Awake()
    {
        slider.onValueChanged.AddListener(onSliderValueChange);
    }

    private void onSliderValueChange(float value)
    {
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * volumeMultiplier);
    }
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

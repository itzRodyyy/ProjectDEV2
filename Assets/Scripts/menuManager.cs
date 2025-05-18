using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class menuManager : MonoBehaviour
{
    public static menuManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("*** Menus ***")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject pauseMenu;

    [Header("*** Popups ***")]
    [SerializeField] GameObject quitPopup;
    [SerializeField] GameObject continuePopup;
    [SerializeField] GameObject newGamePopup;

    [Header("*** Options ***")]
    [SerializeField] string masterVolumeParameter = "MasterVolume";
    [SerializeField] string musicVolumeParameter = "MusicVolume";
    [SerializeField] string sfxVolumeParameter = "SFXVolume";
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] AudioMixer audioMixer;


    [Header("*** Variables ***")]
    GameObject menuActive;
    GameObject prevMenu;
    float timeScaleOrig;
    bool isPaused;
    void Awake()
    {
        instance = this;
        if (mainMenu != null)
            menuActive = mainMenu;
        masterSlider.onValueChanged.AddListener(masterVolumeValueChange);
        musicSlider.onValueChanged.AddListener(musicVolumeValueChange);
        sfxSlider.onValueChanged.AddListener(sfxVolumeValueChange);

        timeScaleOrig = Time.timeScale;
    }

    private void sfxVolumeValueChange(float value)
    {
        audioMixer.SetFloat(sfxVolumeParameter, Mathf.Log10(value) * 20f);
    }

    private void musicVolumeValueChange(float value)
    {
        audioMixer.SetFloat(musicVolumeParameter, Mathf.Log10(value) * 20f);
    }

    private void masterVolumeValueChange(float value)
    {
        audioMixer.SetFloat(masterVolumeParameter, Mathf.Log10(value) * 20f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Cancel()
    {
        menuActive.SetActive(false);
        menuActive = prevMenu;
        menuActive.SetActive(true);
    }

    public void OpenCredits()
    {
        prevMenu = menuActive;
        prevMenu.SetActive(false);
        menuActive = creditsMenu;
        menuActive.SetActive(true);
    }

    public void OpenOptions()
    {
        prevMenu = menuActive;
        prevMenu.SetActive(false);
        menuActive = optionsMenu;
        menuActive.SetActive(true);
    }
    public void QuitQuery()
    {
        prevMenu = menuActive;
        prevMenu.SetActive(false);
        menuActive = quitPopup;
        menuActive.SetActive(true);
    }

    public void NewGameQuery()
    {
        prevMenu = menuActive;
        prevMenu.SetActive(false);
        menuActive = newGamePopup;
        menuActive.SetActive(true);
    }

    public void ContinueQuery()
    {
        prevMenu = menuActive;
        prevMenu.SetActive(false);
        menuActive = continuePopup;
        menuActive.SetActive(true);
    }
}

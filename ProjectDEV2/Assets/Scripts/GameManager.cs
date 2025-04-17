using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject player;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuYouDied;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuShop;
    [SerializeField] public int price;
    [SerializeField] TMP_Text gameGoalCountText;

    GameObject previousMenu;

    public GameObject playerDamageScreen;
    public playerController playerScript;
    public Image playerHPBar;

    public bool isPaused;

    float timeScaleOrig;

    int gameGoal;
    public int currency;

    Resolution[] resolutions;
    int currentResolutionIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        timeScaleOrig = Time.timeScale;

        SetupResolutionDropdown();

        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }

    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount, int cur)
    {
        gameGoal += amount;

        currency += cur;

        if (gameGoal <= 0)
        {
            // You won!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void youDied()
    {
        statePause();
        menuActive = menuYouDied;
        menuActive.SetActive(true);
    }

    public void options()
    {
        if (menuActive == menuOptions)
        {
            menuOptions.SetActive(false);
            if(previousMenu != null)
                previousMenu.SetActive(true);

            menuActive = previousMenu;
        }
        else
        {      
            if (menuActive != null)
                menuActive.SetActive(false);

            previousMenu = menuActive;
            menuOptions.SetActive(true);
            menuActive = menuOptions;
        }
    }

    public void cancelOptions()
    {
        if (menuActive == menuOptions)
        {
            menuOptions.SetActive(false);
            menuActive = previousMenu;
            if (previousMenu != null)
                previousMenu.SetActive(true);

            menuActive = previousMenu;
        }
    }
    void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null)
            return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen: " + isFullscreen);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        Debug.Log("Volume set to: " + volume);
    }

    public void shopOpen()
    {
        statePause();
        menuActive = menuShop;
        menuActive.SetActive(true);
    }
}



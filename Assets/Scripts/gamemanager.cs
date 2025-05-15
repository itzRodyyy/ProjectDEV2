using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum AmmoType
    {
        ThrowingStones,
        Arrows,
        Crossbow,
        _9mm,
        _556mmNATO,
        _50calBMG,
        PlasmaRounds,
        LaserRounds,
        PulseRounds
    }
    public static GameManager instance;
    public GameObject player;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuYouDied;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject menuShopHeal;
    [SerializeField] GameObject menuUpgrade;
    [SerializeField] public int price;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text skillText;

    public GameObject ammo;
    public bool ammoActive;
    public TMP_Text ammoCurr;
    public TMP_Text ammoTotal;
    public ammoStats playerAmmo;

    // Ability Scores
    public TMP_Text strText;
    public TMP_Text dexText;
    public TMP_Text conText;
    public TMP_Text intText;
    public TMP_Text chaText;
    public TMP_Text wisText; 
    

    GameObject previousMenu;

    public GameObject playerDamageScreen;
    public playerController playerScript;
    public GameObject checkPointPopUp;
    public Image playerHPBar;
    public Image playerXPBar;
    [SerializeField] public TMP_Text hpValue;
    [SerializeField] TMP_Text levelText;
    public GameObject playerSpawnPos;

    public bool isPaused;

    float timeScaleOrig;

    int gameGoal;
    public float currency;
    public int level;
    public int skillPoints;
    public float xp;

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
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
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

    public void updateXP(int exp)
    {
        xp += exp * abilityMult(playerScript.stats.intelligence);
        playerXPBar.fillAmount = (float)xp / levelXP(level + 1);
        levelText.text = level.ToString("F0"); 

        if (xp >= levelXP(level + 1))
        {
            xp -= levelXP(level + 1);
            level++;
            skillPoints++;
            playerXPBar.fillAmount = (float)xp / levelXP(level + 1);
            levelText.text = level.ToString("F0");
        }
    }

    int levelXP(int level)
    {
        if (level == 0) return 0;
        return levelXP(level - 1) + (level - 1) * 10;
    }
    public int abilityMod(int ability)
    {
        return (int)(Mathf.Floor((float)(ability - 10) / 2));
    }
    public float abilityMult(int ability)
    {
        return (float)ability / 10;
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
        GameManager.instance.playerScript.UpdateHPUI();
    }

    public void updateGameGoal(int amount)
    {
        gameGoal += amount;
        gameGoalCountText.text = gameGoal.ToString("F0");

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

    public void gunShopOpen()
    {
        statePause();
        menuActive = menuShop;
        menuActive.SetActive(true);
    }

    public void healShopOpen()
    {
        statePause();
        menuActive = menuShopHeal;
        menuActive.SetActive(true);
    }

    public void OpenUpgrades()
    {
        skillText.text = skillPoints.ToString("F0");
        menuActive.SetActive(false);
        menuActive = menuUpgrade;
        menuActive.SetActive(true);
    }

    public void ToggleAmmoUI()
    {
        ammoActive = !ammoActive;
        ammo.SetActive(ammoActive);
    }

    public void UpdateAmmoUI()
    {
        ammoCurr.text = playerScript.currentWeapon.currentAmmo.ToString("F0");
        ammoTotal.text = playerScript.currentWeapon.magSize.ToString("F0");
    }

 
}



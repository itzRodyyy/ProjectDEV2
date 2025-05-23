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
        Bolts,
        _9mm,
        _556mmNATO,
        _50calBMG,
        PlasmaRounds,
        LaserRounds,
        PulseRounds
    }
    public static GameManager instance;
    public GameObject player;

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

    // Scripts
    public playerHPStats hp_stats_script;
    public playerCore core_script;
    public playerInteraction interaction_script;
    public playerMovement movement_script;
    public playerCombat combat_script;

    public GameObject playerDamageScreen;
    public GameObject checkPointPopUp;
    public Image playerHPBar;
    public Image playerXPBar;
    public TMP_Text hpValue;
    [SerializeField] TMP_Text levelText;
    public GameObject interactText;
    public GameObject playerSpawnPos;

    public bool isPaused;

    float timeScaleOrig;

    int gameGoal;
    public float currency;
    public int level;
    public int skillPoints;
    public float xp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        if (GameObject.FindWithTag("Player"))
        {
            player = GameObject.FindWithTag("Player");
            hp_stats_script = player.GetComponent<playerHPStats>();
            core_script = player.GetComponent<playerCore>();
            interaction_script = player.GetComponent<playerInteraction>();
            movement_script = player.GetComponent<playerMovement>();
            combat_script = player.GetComponent<playerCombat>();
        }

        timeScaleOrig = Time.timeScale;


        //fullscreenToggle.isOn = Screen.fullScreen;
        //fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        //volumeSlider.value = AudioListener.volume;
        //volumeSlider.onValueChanged.AddListener(SetVolume);
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        DontDestroyOnLoad(this);
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
            else if (menuActive == menuManager.instance.optionsMenu)
            {
                cancelOptions();
            }
        }
    }

    public void updateXP(int exp)
    {
        xp += exp * abilityMult(hp_stats_script.stats.intelligence);
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
        UpdateHPUI();
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
        if (menuManager.instance.optionsMenu.activeSelf)
        {
            menuManager.instance.ExitOptions();
            menuActive = menuManager.instance.menuActive;
        }
        else
        {
            menuManager.instance.OpenOptionsInGame(menuActive);
            menuActive = menuManager.instance.optionsMenu;
        }
    }

    public void cancelOptions()
    {
        menuActive = menuManager.instance.ExitOptions();
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
        ammoCurr.text = combat_script.currentWeapon.currentAmmo.ToString("F0");
        ammoTotal.text = combat_script.currentWeapon.magSize.ToString("F0");
    }

    public void ShowInteractText(bool _val)
    {
        interactText.SetActive(_val);
    }

    public void UpdateHPUI()
    {
        GameManager.instance.hpValue.text = hp_stats_script.HP.ToString() + "/" + hp_stats_script.MaxHP.ToString();

        GameManager.instance.playerHPBar.fillAmount = (float)hp_stats_script.HP / hp_stats_script.MaxHP;
    }
}
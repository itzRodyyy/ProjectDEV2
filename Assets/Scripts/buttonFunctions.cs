using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }

    public void quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }

    public void options()
    {

        GameManager.instance.options();
    }

    public void cancelOptions()
    {
        GameManager.instance.cancelOptions();
    }
    public void respawn()
    {
        GameManager.instance.core_script.spawnPlayer();
        GameManager.instance.stateUnpause();
    }

    public void increaseHP(int cost)
    {
        if (GameManager.instance.currency >= GameManager.instance.price)
        {
            GameManager.instance.currency -= GameManager.instance.price;

            GameManager.instance.hp_stats_script.MaxHP++;

            GameManager.instance.UpdateHPUI();
        }
        
    }

    public void buyGun(weaponStats weapon)
    {
        if(GameManager.instance.currency >= weapon.cost)
        {
            GameManager.instance.currency -= weapon.cost / GameManager.instance.abilityMult(GameManager.instance.hp_stats_script.stats.charisma);

            weaponStats weaponInstance = Instantiate(weapon);
            weaponInstance.currentAmmo = weaponInstance.magSize;

            inventoryManager.instance.AddItem(weaponInstance);

            GameManager.instance.interaction_script.GetWeaponStats(weaponInstance);
        }
    }

    public void levelUp()
    {
        GameManager.instance.OpenUpgrades();
    }

    public void upgradeAbility(int ability)
    {
        if (GameManager.instance.skillPoints > 0)
        {
            switch (ability)
            {
                case 1:
                    {
                        GameManager.instance.hp_stats_script.stats.strength++;
                        break;               
                    }                        
                case 2:                      
                    {                        
                        GameManager.instance.hp_stats_script.stats.dexterity++;
                        break;               
                    }                        
                case 3:                      
                    {                        
                        GameManager.instance.hp_stats_script.stats.constitution++;
                        break;               
                    }                        
                case 4:                      
                    {                        
                        GameManager.instance.hp_stats_script.stats.intelligence++;
                        break;                
                    }                         
                case 5:                       
                    {                         
                        GameManager.instance.hp_stats_script.stats.charisma++;
                        break;               
                    }                        
                case 6:                      
                    {                        
                        GameManager.instance.hp_stats_script.stats.wisdom++;
                        break;
                    }
                default: { break; }
            }
            GameManager.instance.hp_stats_script.updateStats();
            GameManager.instance.skillPoints--;
            levelUp();
        }
    }

    // Main Menu Buttons
    public void Credits()
    {
        menuManager.instance.OpenCredits();
    }
    public void MainMenuCancel() {
        menuManager.instance.Cancel();
    }

    public void MainMenuQuit()
    {
        menuManager.instance.QuitQuery();
    }
    public void MainMenuContinue()
    {
        menuManager.instance.ContinueQuery();
    }
    public void MainMenuNewGame()
    {
        menuManager.instance.NewGameQuery();
    }

    public void MainMenuOptions()
    {
        menuManager.instance.OpenOptions();
    }

    public void PlayAudio(AudioClip clip)
    {
        AudioSource aud = gameObject.GetComponent<AudioSource>();
        aud.PlayOneShot(clip);
    }

    public void NewGame()
    {
        SceneLoader.LoadScene(1);
    }
}

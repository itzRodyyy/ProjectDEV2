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
        GameManager.instance.playerScript.spawnPlayer();
        GameManager.instance.stateUnpause();
    }

    public void increaseHP(int cost)
    {
        if (GameManager.instance.currency >= GameManager.instance.price)
        {
            GameManager.instance.currency -= GameManager.instance.price;

            GameManager.instance.playerScript.MaxHP++;

            GameManager.instance.playerScript.UpdateHPUI();
        }
        
    }

    public void buyGun(weaponStats weapon)
    {
        if(GameManager.instance.currency >= weapon.cost)
        {
            GameManager.instance.currency -= weapon.cost / GameManager.instance.abilityMult(GameManager.instance.playerScript.stats.charisma);

            weaponStats weaponInstance = Instantiate(weapon);
            weaponInstance.currentAmmo = weaponInstance.magSize;

            inventoryManager.instance.AddItem(weaponInstance);

            GameManager.instance.playerScript.GetWeaponStats(weaponInstance);
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
                        GameManager.instance.playerScript.stats.strength++;
                        break;
                    }
                case 2:
                    {
                        GameManager.instance.playerScript.stats.dexterity++;
                        break;
                    }
                case 3:
                    {
                        GameManager.instance.playerScript.stats.constitution++;
                        break;
                    }
                case 4:
                    {
                        GameManager.instance.playerScript.stats.intelligence++;
                        break;
                    }
                case 5:
                    {
                        GameManager.instance.playerScript.stats.charisma++;
                        break;
                    }
                case 6:
                    {
                        GameManager.instance.playerScript.stats.wisdom++;
                        break;
                    }
                default: { break; }
            }
            GameManager.instance.playerScript.updateStats();
            GameManager.instance.skillPoints--;
            levelUp();
        }
    }
}

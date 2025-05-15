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
            GameManager.instance.currency -= weapon.cost;

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

    public void upgradeHealth()
    {
        if (GameManager.instance.skillPoints > 0)
        {
            GameManager.instance.playerScript.MaxHP++;
            GameManager.instance.skillPoints--;
            levelUp();
        }
    }
    public void upgradeSpeed()
    {
        if (GameManager.instance.skillPoints > 0)
        {
            GameManager.instance.playerScript.moveSpeed++;
            GameManager.instance.skillPoints--;
            levelUp();
        }
    }
    public void upgradeJump()
    {
        if (GameManager.instance.skillPoints > 0)
        {
            GameManager.instance.playerScript.jumpSpeed++;
            GameManager.instance.skillPoints--;
            levelUp();
        }
    }
}

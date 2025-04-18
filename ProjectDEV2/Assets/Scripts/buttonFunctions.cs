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

    public void increaseHP(int cost)
    {
        if (GameManager.instance.currency >= GameManager.instance.price)
        {
            GameManager.instance.playerScript.HP += 1;
            GameManager.instance.playerScript.UpdateHPUI();
            GameManager.instance.currency -= GameManager.instance.price;
        }
    }
}

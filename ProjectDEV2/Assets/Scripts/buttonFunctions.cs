using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip buttonClick;
    [Range(0, 1)][SerializeField] float buttonClickVolume;
    [Range(0, 1)][SerializeField] float buttonDelay;

    public void resume()
    {
        PlayButtonSound();
        GameManager.instance.stateUnpause();
    }

    public void restart()
    {
        StartCoroutine(DelayedAction(() => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }));
    }

    public void quit()
    {
        StartCoroutine(DelayedAction(() => {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
        }));
    }


    public void options()
    {
        PlayButtonSound();
        GameManager.instance.options();
    }

    public void cancelOptions()
    {
        PlayButtonSound();
        GameManager.instance.cancelOptions();
    }

<<<<<<< Updated upstream

=======
    public void increaseHP(int cost)
    {
        PlayButtonSound();
        if (GameManager.instance.currency >= GameManager.instance.price)
        {
            GameManager.instance.playerScript.HP += 1;
            GameManager.instance.playerScript.UpdateHPUI();
            GameManager.instance.currency -= GameManager.instance.price;
        }
    }

    public void inventory()
    {
        PlayButtonSound();
        inventorymanager.instance.inventory();
    }

    private void PlayButtonSound()
    {
        aud.PlayOneShot(buttonClick, buttonClickVolume);
    }

    private IEnumerator DelayedAction(System.Action action)
    {
        if (GameManager.instance.isPaused)
            GameManager.instance.stateUnpause();
        PlayButtonSound();
        yield return new WaitForSeconds(buttonDelay);
        action.Invoke();
    }
>>>>>>> Stashed changes
}

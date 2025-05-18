using UnityEngine;

public class menuManager : MonoBehaviour
{
    public static menuManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject optionsMenu;

    [SerializeField] GameObject quitPopup;
    [SerializeField] GameObject continuePopup;
    [SerializeField] GameObject newGamePopup;

    GameObject menuActive;
    GameObject prevMenu;
    void Awake()
    {
        instance = this;
        menuActive = mainMenu;
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

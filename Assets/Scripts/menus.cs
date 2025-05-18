using UnityEngine;

[CreateAssetMenu(fileName = "main_menus", menuName = "Scriptable Objects/main_menus")]
public class main_menus : ScriptableObject
{
    [Header("*** Menu Options ***")]
    public GameObject MainMenu;
    public GameObject CreditsMenu;
    public GameObject OptionsMenu;

    [Header("*** Popups ***")]
    public GameObject NewGamePopup;
    public GameObject ContinuePopup;
    public GameObject QuitPopup;
}

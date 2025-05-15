using UnityEngine;

[CreateAssetMenu(fileName = "playerStats", menuName = "Scriptable Objects/playerStats")]
public class playerStats : ScriptableObject
{
    [Header("*** Ability Scores ***")]
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    public int charisma;
    public int wisdom;
}

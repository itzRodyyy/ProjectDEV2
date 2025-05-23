using UnityEngine;

[CreateAssetMenu(fileName = "abilityScores", menuName = "Scriptable Objects/abilityScores")]
public class abilityScores : ScriptableObject
{
    [Header("*** Ability Scores ***")]
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    public int charisma;
    public int wisdom;
}

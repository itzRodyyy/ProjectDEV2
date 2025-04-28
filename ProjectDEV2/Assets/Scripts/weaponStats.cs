using UnityEngine;

[CreateAssetMenu]
public class weaponStats : ScriptableObject
{
    [Header("--- Basic Components ---")]
    public bool isMelee;
    public bool isAutomatic;
    public GameObject weaponModel;

    [Header("--- Weapon Stats ---")]
    // Universal
    [Range(1, 10)] public int weaponDamage;
    [Range(0.1f, 3)] public float attackRate;
    [Range(5, 1000)] public int range;

    // Ranged Only
    public int currentAmmo;
    [Range(1, 50)] public int magSize;
    public Transform shootPos;
    
}

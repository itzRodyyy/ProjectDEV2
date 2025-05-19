using UnityEngine;

[CreateAssetMenu(fileName = "player", menuName = "Scriptable Objects/player")]
public class player : ScriptableObject
{
    public int moveSpeed;
    public int sprintMod;
    public int jumpSpeed;
    public int jumpMax;
    public int gravity;
    public float crouchHeightMod;
    public float crouchSpeedMod;

    public weaponStats currentWeapon;
}

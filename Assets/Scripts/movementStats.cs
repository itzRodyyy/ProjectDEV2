using UnityEngine;

[CreateAssetMenu(fileName = "movementStats", menuName = "Scriptable Objects/movementStats")]
public class movementStats : ScriptableObject
{
    public int moveSpeed;
    public int sprintMod;
    public int jumpSpeed;
    public int jumpMax;
    public int gravity;
    public float crouchHeightMod;
    public float crouchSpeedMod;
}

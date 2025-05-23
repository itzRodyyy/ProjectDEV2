using UnityEngine;

public class Door : MonoBehaviour, iInteract
{
    [SerializeField] Animator doorAnim;

    public void onInteract()
    {
        if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName("DoorOpen"))//if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
        {
            doorAnim.ResetTrigger("Open");
            doorAnim.SetTrigger("Close");
        }

        if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName("DoorClose"))
        {
            doorAnim.ResetTrigger("Close");
            doorAnim.SetTrigger("Open");
        }
    }

    
}

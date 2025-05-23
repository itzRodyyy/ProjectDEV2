using UnityEngine;

public class lever : MonoBehaviour, iInteract
{
    [SerializeField] Animator leverAnim;

    public void onInteract()
    {
        if (leverAnim.GetCurrentAnimatorStateInfo(0).IsName("leverSwitch"))
        {
            leverAnim.ResetTrigger("on");
            leverAnim.SetTrigger("off");
        }

        if (leverAnim.GetCurrentAnimatorStateInfo(0).IsName("leverSwitchAlt"))
        {
            leverAnim.ResetTrigger("off");
            leverAnim.SetTrigger("on");
        }
    }
}

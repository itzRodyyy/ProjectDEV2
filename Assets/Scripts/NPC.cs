using UnityEngine;

public class NPC : MonoBehaviour, iInteract
{
    bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void onInteract()
    {
        //GameManager.instance.dialogueOn();
    }
}

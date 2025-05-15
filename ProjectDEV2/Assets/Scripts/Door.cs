using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //If you already have your own raycast script, feel free to integrate whatever you need from this door script into your current raycast script :)

    //Distance from which the player can interact with the door
    public float interactionDistance;

    //The text that appears to let you know you can interact with the door
    public GameObject intText;
    public GameObject labelText;

    //The names of the door open and door close animations
    public string doorOpenAnimName, doorCloseAnimName;

    //The door open and door close sounds
    //public AudioClip doorOpen, doorClose;


    //The Update() void is where stuff occurs every frame
    void Update()
    {
        //A ray is created which will shoot forward from the player's camera
        Ray ray = new Ray(transform.position, transform.forward);

        //RaycastHit variable, which is used to get info back from whatever the raycast hits
        RaycastHit hit;

        //If the raycast hits something
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            //If the object the raycast hits is tagged as door
            if (hit.collider.gameObject.tag == "door")
            {
                //A GameObject variable is created for the door's main parent object
                GameObject doorParent = hit.collider.transform.root.gameObject;
                Animator doorAnim = doorParent.GetComponent<Animator>();
                intText.SetActive(true);
                labelText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
                    {

                        doorAnim.SetTrigger("Close");
                    }

                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorCloseAnimName))
                    {

                        doorAnim.SetTrigger("Open");
                    }

                }
            }
            //else, if not looking at the door
            else
            {
                //The interaction text is disabled
                intText.SetActive(false);
                labelText.SetActive(false);
            }
        }
        //else, if not looking at anything
        else
        {
            //The interaction text is disabled
            intText.SetActive(false);
            labelText.SetActive(false);
        }
    }
}
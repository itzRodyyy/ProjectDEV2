using UnityEngine;

public class DisapearingPlatformms : MonoBehaviour
{
    [SerializeField] float destroyTime;
    bool playerInRange;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            timer += Time.deltaTime;
        }

        if (timer > destroyTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
}

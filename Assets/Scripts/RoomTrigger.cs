using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] bool spawnPos2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.currentLevel++;
            other.transform.position = (spawnPos2) ? GameManager.instance.playerSpawnPos2.transform.position : GameManager.instance.playerSpawnPos3.transform.position;
            GameManager.instance.respawnPos = (spawnPos2) ? GameManager.instance.playerSpawnPos2 : GameManager.instance.playerSpawnPos3;
        }
    }
}

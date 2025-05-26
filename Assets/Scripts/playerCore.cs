using UnityEngine;

public class playerCore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioSource aud;
    [SerializeField] CharacterController controller;
    public static playerCore playerInstance;

    private void Awake()
    {
        if (playerInstance == null)
        {
            playerInstance = this;
        }
        else if (playerInstance != this)
        {
            Destroy(playerInstance);
        }
    }
    void Start()
    {
        spawnPlayer(GameManager.instance.playerSpawnPos1.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnPlayer(Vector3 position) // Core
    {
        controller.transform.position = position;

        GameManager.instance.hp_stats_script.HP = GameManager.instance.hp_stats_script.MaxHP;
        GameManager.instance.UpdateHPUI();
    }
}

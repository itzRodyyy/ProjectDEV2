using UnityEngine;

public class playerCore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioSource aud;
    [SerializeField] CharacterController controller;
    void Start()
    {
        spawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnPlayer() // Core
    {
        controller.transform.position = GameManager.instance.playerSpawnPos.transform.position;

        GameManager.instance.hp_stats_script.HP = GameManager.instance.hp_stats_script.MaxHP;
        GameManager.instance.UpdateHPUI();
    }
}

using System.Collections;
using UnityEngine;

public class playerHPStats : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] AudioSource aud;
    public int HP; // HP & Stats
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    public int MaxHP; // HP & Stats
    public abilityScores stats; // HP & Stats

    int baseSpeed; // HP & Stats
    int baseHP; // HP & Stats

    void Awake()
    {
        baseHP = HP;
        baseSpeed = GameManager.instance.movement_script.moveStats.moveSpeed;
        updateStats();
    }
    void Start()
    {
        GameManager.instance.updateXP(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TakeDamage(int amount) // HP & Stats
    {
        HP -= amount;
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        GameManager.instance.UpdateHPUI();
        StartCoroutine(flashDamage());
        if (HP <= 0)
        {
            GameManager.instance.youDied();
        }
    }

    IEnumerator flashDamage() // HP & Stats
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void updateStats() // HP & Stats
    {
        GameManager.instance.strText.text = stats.strength.ToString("F0");
        GameManager.instance.dexText.text = stats.dexterity.ToString("F0");
        GameManager.instance.conText.text = stats.constitution.ToString("F0");
        GameManager.instance.intText.text = stats.intelligence.ToString("F0");
        GameManager.instance.chaText.text = stats.charisma.ToString("F0");
        GameManager.instance.wisText.text = stats.wisdom.ToString("F0");
        GameManager.instance.movement_script.moveStats.moveSpeed = baseSpeed + GameManager.instance.abilityMod(stats.dexterity);
        MaxHP = baseHP + GameManager.instance.abilityMod(stats.constitution);
        GameManager.instance.UpdateHPUI();
    }
}

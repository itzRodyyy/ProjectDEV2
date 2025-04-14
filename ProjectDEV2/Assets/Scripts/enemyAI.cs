using System.Collections;
using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] Renderer model;

    Color modelOrig;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        modelOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());
        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = modelOrig;
    }
}

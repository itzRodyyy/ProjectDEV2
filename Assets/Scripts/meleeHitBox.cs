using UnityEngine;
using System.Collections;

public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float activeTime;
    [SerializeField] string targetTag = "Player";
    [SerializeField] float knockbackForce = 8f;

    Collider hitbox;

    void Awake()
    {
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false;
    }

    public void ActivateHitbox()
    {
        StartCoroutine(EnableHitbox());
    }

    IEnumerator EnableHitbox()
    {
        hitbox.enabled = true;
        yield return new WaitForSeconds(activeTime);
        hitbox.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            IDamage target = other.GetComponent<IDamage>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }

        playerController player = other.GetComponent<playerController>();
        if (player != null)
        {
            player.ApplyKnockback(transform.position, knockbackForce);
        }
    }
}

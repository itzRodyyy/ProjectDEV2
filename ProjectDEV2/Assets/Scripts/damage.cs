using System.Collections;
using UnityEngine;

public class damage : MonoBehaviour
{
    enum type { moving, homing, stationary, DOT};
    [SerializeField] type damageType;
    [SerializeField] Rigidbody rb; 
    
    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] LayerMask self;

    bool isDamaging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (damageType == type.moving || damageType == type.homing)
        {
            Destroy(gameObject, destroyTime);
            if (damageType == type.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (damageType == type.moving || damageType == type.homing)
        {
            Debug.DrawRay(transform.position, transform.forward * speed * Time.deltaTime);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, speed * Time.deltaTime, ~self) || Physics.Raycast(transform.position, -transform.forward, out hit, (speed * Time.deltaTime) / 4, ~self))
            {

                IDamage dmg = hit.collider.gameObject.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.TakeDamage(damageAmount);
                }
                Debug.Log(hit.collider);
                Destroy(gameObject);

            }
            if (damageType == type.homing)
            {
                rb.linearVelocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider victim)
    {
        Debug.Log(victim);
        if (damageType != type.DOT)
        {
            if (damageType == type.moving || damageType == type.homing)
            {
                Destroy(gameObject);
            }
            IDamage dmg = victim.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(damageAmount);
            }
        }
    }
    private void OnTriggerStay(Collider victim)
    {
        if (victim.isTrigger)
            return;
        if (damageType == type.DOT)
        {
            IDamage dmg = victim.GetComponent<IDamage>();
            if (dmg != null && !isDamaging)
            {
                StartCoroutine(damageOT(dmg));
            }
        }
    }

    IEnumerator damageOT(IDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}

using UnityEngine;

public class damage : MonoBehaviour
{
    enum type { moving, homing, stationary, DOT};
    [SerializeField] type damageType;
    [SerializeField] Rigidbody rb; 
    
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] LayerMask self;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (damageType == type.moving)
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
        if (damageType == type.moving)
        {
            Debug.DrawRay(transform.position, transform.forward * speed * Time.deltaTime);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, speed * Time.deltaTime, ~self) || Physics.Raycast(transform.position, -transform.forward, out hit, speed * Time.deltaTime, ~self))
            {

                IDamage dmg = hit.collider.gameObject.GetComponent<IDamage>();
                if (dmg != null)
                {
                    dmg.TakeDamage(damageAmount);
                }
                Debug.Log(hit.collider);
                Destroy(gameObject);

            }
        }
    }

    private void OnTriggerEnter(Collider victim)
    {
        Debug.Log(victim);
        if (damageType == type.moving)
        {
            Destroy(gameObject);
            IDamage dmg = victim.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(damageAmount);
            }
        }
    }
}

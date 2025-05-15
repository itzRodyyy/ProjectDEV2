using UnityEngine;

public class ammoPickup : MonoBehaviour
{
    [SerializeField] int amount;
    [SerializeField] GameManager.AmmoType ammoType;
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
        iAmmoPickup pickupable = other.GetComponent<iAmmoPickup>();
        if (pickupable != null)
        {
            pickupable.addAmmo(amount, ammoType);
            Destroy(gameObject);
        }
    }
}

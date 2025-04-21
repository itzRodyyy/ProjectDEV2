using System.Collections;
using UnityEngine;

public class gun : MonoBehaviour, iWeapon
{
    // Gun Stats
    [SerializeField] int damage;
    [SerializeField] public int magSize;
    [SerializeField] float fireRate;
    [SerializeField] float reloadSpeed;
    [SerializeField] float distance;
    [SerializeField] bool automatic;


    // Trail
    [SerializeField] Color colour;
    [SerializeField] Transform shootPos;
    [SerializeField] float trailThickness;
    [SerializeField] float trailDuration;

    LineRenderer lineRenderer;
    public int ammo;
    float fireTimer;
    Vector3 endPt;
    bool isReloading;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammo = magSize;
        lineRenderer = new GameObject().AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = colour;
        lineRenderer.endColor = colour;
        lineRenderer.startWidth = trailThickness;
        lineRenderer.endWidth = trailThickness;
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;

    }

    public void Attack()
    {
        if (automatic)
        {
            if (Input.GetButton("Fire1") && fireTimer >= fireRate && ammo > 0)
            {
                fireTimer = 0;
                ammo--;
                Debug.Log(ammo);
                checkCollision();
                StartCoroutine(bulletTrail(shootPos.position, endPt));

            }
        }
        else
        {
            if (Input.GetButtonUp("Fire1") && fireTimer >= fireRate && ammo > 0)
            {
                fireTimer = 0;
                ammo--;
                Debug.Log(ammo);
                checkCollision();
                StartCoroutine(bulletTrail(shootPos.position, endPt));
            }
        }

        if (ammo == 0 && !isReloading)
        {
            StartCoroutine(reload());
        }
    }

    private void checkCollision()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position, transform.parent.forward, out hit, distance, ~LayerMask.GetMask("Player")))
        {
            endPt = hit.point;
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
            Debug.Log(hit.collider);
        }
        else
        {
            endPt = transform.parent.position + transform.parent.forward * distance;
        }
    }

    IEnumerator bulletTrail(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, start);
        lineRenderer.enabled = true;

        float duration = 0.1f; // adjust this to control the speed of the bullet
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            lineRenderer.SetPosition(1, Vector3.Lerp(start, end, t));
            yield return null;
        }

        lineRenderer.enabled = false;
    }

    IEnumerator reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadSpeed);
        ammo = magSize;
        isReloading = false;
    }
}

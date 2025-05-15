using UnityEngine;
using System.Collections;

public class zipLine : MonoBehaviour
{
    public Transform zipTrans;
    [SerializeField] zipLine targetZip;
    [SerializeField] float zipSpeed = 5f;
    [SerializeField] float zipScale = .2f;
    [SerializeField] float arrivalThreshold = 0.4f;
    [SerializeField] LineRenderer cable;

    private bool zipping = false;
    private GameObject localZip;
    private void Awake()
    {
        cable.SetPosition(0, zipTrans.position);
        cable.SetPosition(1, targetZip.zipTrans.position);
    }

    private void Update()
    {
        if (!zipping || localZip == null)
        {
            return;
        }

        localZip.GetComponent<Rigidbody>().AddForce((targetZip.zipTrans.position - zipTrans.position).normalized * Time.deltaTime, ForceMode.Acceleration);

        if (Vector3.Distance(localZip.transform.position, targetZip.zipTrans.position) <= arrivalThreshold)
        {
            ResetZipline();
        }
    }

    public void StartZipLine(GameObject player)
    {
        if (zipping) 
        {
            return;
        }

        localZip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        localZip.transform.position = zipTrans.position;
        localZip.transform.localScale = new Vector3(zipScale, zipScale, zipScale);
        localZip.AddComponent<Rigidbody>().useGravity = false;
        localZip.GetComponent<Collider>().isTrigger = true;

        GameManager.instance.player.GetComponent<Rigidbody>().useGravity = false;
        GameManager.instance.player.GetComponent<Rigidbody>().isKinematic = true;
        GameManager.instance.player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GameManager.instance.player.transform.parent = localZip.transform;
        zipping = true;
    }

    public void ResetZipline()
    {
        if (!zipping)
        {
            return;
        }

        GameObject player = localZip.transform.GetChild(0).gameObject;

        GameManager.instance.player.GetComponent<Rigidbody>().useGravity = true;
        GameManager.instance.player.GetComponent<Rigidbody>().isKinematic = false;
        GameManager.instance.player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GameManager.instance.player.transform.parent = null;
        Destroy(localZip);

        localZip = null;
        zipping = false;
        Debug.Log("Zipline reset");
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}

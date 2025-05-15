using System.Collections;
using UnityEngine;

public class zipLine : MonoBehaviour, Iinteract
{
    [SerializeField] LineRenderer lineRenderer;
    public Transform startPoint;
    public Transform endPoint;
    public float ziplineSpeed = 5f;

    private bool isPlayerOnZipline = false;
    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);
    }
    public void onInteract()
    {
        if (!isPlayerOnZipline)
        {
            StartCoroutine(ZiplineRide());
        }
    }

    IEnumerator ZiplineRide()
    {
        isPlayerOnZipline = true;
        float journeyLength = Vector3.Distance(startPoint.position, endPoint.position);
        float journeyTime = journeyLength / ziplineSpeed;
        float elapsedTime = 0;

        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        while (elapsedTime < journeyTime)
        {
            GameManager.instance.player.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / journeyTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GameManager.instance.player.transform.position = endPos;
        isPlayerOnZipline = false;
    }
}
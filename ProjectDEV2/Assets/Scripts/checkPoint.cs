using UnityEngine;
using System.Collections;

public class checkPoint : MonoBehaviour
{
    [SerializeField] Renderer model;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(checkPointFeedback());
        }
    }

    IEnumerator checkPointFeedback()
    {
        model.material.color = Color.red;
        GameManager.instance.checkPointPopUp.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameManager.instance.checkPointPopUp.SetActive(false);
        model.material.color = Color.white;
    }
}

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Showcase");
        }
    }

    public static void LoadScenes()
    {
        SceneManager.LoadScene("mainScene");
        SceneManager.LoadScene("sandboxJackie", LoadSceneMode.Additive);
        SceneManager.LoadScene("MedievalScene", LoadSceneMode.Additive);
        SceneManager.LoadScene("FutureScene", LoadSceneMode.Additive);
    }
}

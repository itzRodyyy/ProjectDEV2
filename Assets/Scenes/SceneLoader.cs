using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
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

    public static void LoadScene(int index)
    {
        switch(index)
        {
            case 1:
                SceneManager.LoadScene("PresentScene");
                break;
            case 2:
                SceneManager.LoadScene("MedievalScene");
                break;
            case 3:
                SceneManager.LoadScene("FutureScene");
                break;
            default:
                break;
        }
    }
}

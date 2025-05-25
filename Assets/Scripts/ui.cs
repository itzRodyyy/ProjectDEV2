using UnityEngine;

public class ui : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static ui Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

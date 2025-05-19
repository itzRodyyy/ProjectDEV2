using UnityEngine;

public class oPersist : MonoBehaviour
{
    [SerializeField] bool objectPersist;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (objectPersist)
            DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

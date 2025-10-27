using UnityEngine;

public class HomingAimCS : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void doLockOn(bool lockonFlag)
    {
        this.gameObject.SetActive(lockonFlag);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

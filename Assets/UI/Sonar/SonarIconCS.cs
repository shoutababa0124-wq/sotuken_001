using UnityEngine;

public class SonarIconCS : MonoBehaviour
{
    float time = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void doInit()
    {
        this.gameObject.SetActive(false);
    }

    public void doSonar(bool flag,Vector3 pos)
    {
        transform.localPosition = pos;
        this.gameObject.SetActive(flag);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

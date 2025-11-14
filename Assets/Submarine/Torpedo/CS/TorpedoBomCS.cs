using UnityEngine;

public class TorpedoBomCS : MonoBehaviour
{
    float destroyTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroyTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        destroyTime += Time.deltaTime;
        if(destroyTime > 0.5f)
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class SonarImageCS : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void doRotSonar(Vector3 playerRot)
    {
        transform.localEulerAngles = new Vector3(0, 0, playerRot.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
